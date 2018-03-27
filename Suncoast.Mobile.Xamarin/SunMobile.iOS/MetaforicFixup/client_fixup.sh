#!/bin/bash
VERSION="Xam.1.0"
set -x
INAUTH=`dirname "$0"`

BUILD_CONFIG=
BUILD_PLATFORM=
DIRECTORY="$INAUTH/tmp"
# clean out the log dir
rm -rf "$DIRECTORY"
mkdir -p "$DIRECTORY"

while [ $# -gt 0 ]; do
option="$1"
shift

case $option in
    -c|--config)
        BUILD_CONFIG=$1
        if [ -z "$BUILD_CONFIG" ]; then
          echo 'Build configuration not defined, add -c|--config ${ProjectConfigName} as an argument.'
          exit 1
        fi
        shift
        ;;
    -p|--platform)
        BUILD_PLATFORM=$1
        if [ -z "$BUILD_PLATFORM" ]; then
          echo 'Build platform not defined, add -p|--platform ${ProjectConfigPlat} as an argument.'
          exit 1
        fi
        shift
        ;;
    *)
        echo "Unknown option '$option' specified."
        usage
        exit 1
        ;;
esac
done

project_path=$(pwd)
project_name=$(basename "$project_path")
obj_output_path="$project_path/obj/$BUILD_PLATFORM/$BUILD_CONFIG"
mtouch_cache_path="$obj_output_path/mtouch-cache"
output_path=$(xpath $project_name.csproj '//PropertyGroup[contains(@Condition,"'" '$BUILD_CONFIG|$BUILD_PLATFORM' "'")]/OutputPath/text()' 2>/dev/null | sed 's/\\/\//g')
extra_args=$(xpath $project_name.csproj '//PropertyGroup[contains(@Condition,"'" '$BUILD_CONFIG|$BUILD_PLATFORM' "'")]/CodesignExtraArgs/text()' 2>/dev/null)
assembly_name=SunMobileiOS
entitlements=$(xpath $project_name.csproj '//PropertyGroup[contains(@Condition,"'" '$BUILD_CONFIG|$BUILD_PLATFORM' "'")]/CodesignEntitlements/text()' 2>/dev/null | cut -d"." -f1)
application_path="$project_path/$output_path/$assembly_name.app"
application_binary="$application_path/$assembly_name"
codesign_info=$(codesign --display -r- "$application_path")
signing_identity=$(echo $codesign_info | awk -v FS="(= | and)" '{print $4}')
signing_data=$(security find-identity -v -p codesigning | grep -m 1 "${signing_identity}$")
codesign_hash=$(echo ${signing_data} | cut -d" " -f2)

# codesign_hash=“9C5F6EB17C89A2AF6EF9B0E9ACDF668D6E6DA17E”

recodesign()
{
  codesign -v --force --sign $codesign_hash --entitlements "$obj_output_path/$entitlements.xcent" "$application_binary" $extra_args
}

fixupBinary()
{
    local file="$1"
    local arch=$2
    local targetenv
    local global="$INAUTH/metafortress.conf"

    case ${arch} in
	armv7*)
	    targetenv=darwin_gnu_arm;
	    ;;
	arm64)
	    targetenv=darwin_gnu_arm_64;
	    ;;
	*)
	    echo ERROR: Unsupported architecture ${arch}
	    exit 1
    esac

    local dbdir="${DIRECTORY}/${arch}.db"
    local db="${INAUTH}/metafortress.dat.${arch}"
    rm -rf "${dbdir}"
    mkdir "${dbdir}"

    echo "$INAUTH/MFFixup" --config "$INAUTH/metafortress.config" --database ${db} --global ${global} --target ${targetenv} "${file}" "$INAUTH" >> "$DIRECTORY/log.txt" 2>&1
    echo "$INAUTH/MFFixup" --config "$INAUTH/metafortress.config" --database ${db} --global ${global} --target ${targetenv} "${file}" "$INAUTH"
    "$INAUTH/MFFixup" --config "$INAUTH/metafortress.config" --database "${db}" --global "${global}" --target ${targetenv} "${file}" "$INAUTH" >> "$DIRECTORY/fixup-${arch}.log" 2>&1

    if [[ -e "${file}_unfixedup" && -e "${file}.mfa" && -e "${file}.szchks" ]]; then
        mv -v "${file}_unfixedup" "${file}.mfa" "${file}.szchks" "${dbdir}"
    fi
}

debugFixup()
{
    cp "$INAUTH/metafortress.dat.armv7.d" "$INAUTH/metafortress.dat.armv7"
    cp "$INAUTH/metafortress.dat.armv7s.d" "$INAUTH/metafortress.dat.armv7s"
    cp "$INAUTH/metafortress.dat.arm64.d" "$INAUTH/metafortress.dat.arm64"
    cp "$INAUTH/metafortress.config.d" "$INAUTH/metafortress.config"
    fixupArchs
    return $?
}

productionFixup()
{
    cp "$INAUTH/metafortress.dat.armv7.p" "$INAUTH/metafortress.dat.armv7"
    cp "$INAUTH/metafortress.dat.armv7s.p" "$INAUTH/metafortress.dat.armv7s"
    cp "$INAUTH/metafortress.dat.arm64.p" "$INAUTH/metafortress.dat.arm64"
    cp "$INAUTH/metafortress.config.p" "$INAUTH/metafortress.config"
    fixupArchs
    return $?
}

fixupArchs()
{
    lipo_output=`lipo -info "$application_binary"`
    lipo_arch_list=${lipo_output##*:}
    arch_array=()
    for lipo_arch in $lipo_arch_list; do
        arch_array+=($lipo_arch)
    done
    if [ ${#arch_array[@]} -gt 1 ]; then
        echo "Fat binary"
        for arch in "${arch_array[@]}"; do
            echo "Fixing up ${arch}"
            lipo -thin $arch "$application_binary" -output "${DIRECTORY}/${arch}"
            fixupBinary "${DIRECTORY}/${arch}" $arch
            fixup_status=$?
            if [[ $fixup_status -ne 0 ]]; then
                break
            fi
      	    lipo "$application_binary" -replace $arch "${DIRECTORY}/${arch}" -output "$application_binary"
      	done
    else
        echo "Thin binary"
        fixupBinary "$application_binary" $arch
      	fixup_status=$?
    fi
    return $fixup_status
}

rm -rf "$INAUTH/metafortress.config" "$INAUTH/metafortress.dat.arm64" "$INAUTH/metafortress.dat.armv7" "$INAUTH/metafortress.dat.armv7s"

# Search $ProjectDir/obj/$ProjectConfigPlat/$ProjectConfigName/mtouch-cache for InAuth libraries:
# Check for non-protected library
unprotected_lib=$(ls "$mtouch_cache_path" | grep "libInMobile_ios[^[:space:]]*u\.a")
if [ ! -z "$unprotected_lib" ]; then
    echo "Warning: InAuth unprotected library ["$unprotected_lib"] detected, client_fixup_xamarin.sh cannot be applied to an unprotected InAuth library, skipping."
    exit 0;
fi

# Check for debug protected library
debug_protected_lib=$(ls "$mtouch_cache_path" | grep "libInMobile_ios[^[:space:]]*dm\.a")
if [ ! -z "$debug_protected_lib" ]; then
    echo "InAuth debug protected library [$debug_protected_lib] detected in project."
    debugFixup
    recodesign
    exit 0
fi

# Check for production protected library
production_lib=$(ls "$mtouch_cache_path" | grep "libInMobile_ios[^[:space:]]*pm\.a")
if [ ! -z "$production_lib" ]; then
    echo "InAuth production protected library [$production_lib] detected in project."
    productionFixup
    recodesign
    exit 0
fi

echo "Warning: InAuth library not detected in project, unable to apply client_fixup.sh"
exit 0
