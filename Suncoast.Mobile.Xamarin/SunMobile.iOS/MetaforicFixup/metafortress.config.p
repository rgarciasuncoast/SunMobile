<?xml version="1.0" encoding="utf-8"?>
<Config version="1">
  <UseAnalysisServer value="true"/>
  <AnalysisServerHostname value="10.201.0.110"/>
  <AnalysisServerPort value="8082"/>
  <AnalysisServerPacketsPerBuffer value="50" />
  <AnalysisServerRequestedBuffers value="512" />
  <FocusedZoneProximityTolerance value="102400" />
  <FocusedZoneMinImportance value="10" />
  <FocusedZoneChecksumImportance value="10" />
  

  <AnalysisDensity value="100" />
  <AnalysisSaturation value="20" />
  <AvoidInlineAnalysis value="true" />
  <CaseModel value="incidence" />
  <CaseSensitivity value="high" />
  <CasePrediction value="True" />

  <!-- Instrumentation density -->
  <ChecksCryptoKey value="Optional Checks Key" />
  <CheckDensity value="100" />
  <MinChecksPerFunction value="1" />
  <MaxChecksPerFunction value="1" />
  <UseManagedChecks value="False" />
  <ManagedCheckPeriod value="3" />
  <KByteRate value="512" />
  <KByteMaxCheck value="512" />

  <!-- SuperChecks (checksum entire binary) -->
  <NumSuperChecks value="10" />
  <AutoConfigureSuperChecks value="False" />

  <!-- Common settings -->
  <UseLogicConflation value="True" />
  <UseNondestructiveResponses value="False" />

  <!-- Callbacks -->
  <UseCallbackFunctions value="True"/>
  <UseCallbackAuthentication value="False" />
  <CallbackInjectRate value="1.0" />
  
  <!-- Crypto -->
  <GlobalCryptoKey value="default" />
  <GlobalCryptoComplexity value="15" />

  <!-- Section Encryption -->
  <EnableSectionEncryption value="False" />
  <SectionEncryptionMethod value="add" />
  <SectionEncryptionSubject value="__cstring" />
  <SectionEncryptionDelimiter value="" />

  <!-- Debugging -->
  <UseDiagnosticChecks value="False" />
  <UseDiagnosticResponses value="False" />
  <MakeChecksFail value="False" />
  <EnableTraceLogging value="False" />
  <AllowDebugging value="False"/>

  <!-- Reporting -->
  <ToolVerbosity value="7" />
  <GenerateMFAfile value="True" />
  <ObfuscateReports value="False" />
  <UseDataObfuscation value="False" />
	<EnableTraceLogging value="False" />
	<TraceLoggingPath value="protected_trace.log" />
  <!-- Hazards -->
  <HazardousAssembly value="False" />
  <HazardousExceptions value="False" />
  <PreventInlinedChecks value="False" />

  <!-- Optimisation -->
  <DemoteUnnecessaryInlines value="False" />

  <!-- Troubleshooting -->
  <EnableMetaSST value="True" />
  <EnableInstrumentation value="True" />
  <EnableSourceSync value="True" />
  <UseInterleaving value="True" />
  <UseFalsePositives value="True" />

  <!-- Advanced settings -->
  <UseCloaking value="False" />
  <UseJunkCode value="False" />
  <ExcludeHighFreqFiles value="0" />

  <!-- Finetuning -->
  <StampRate value="0.01" />
  <FalsePositiveRate value="0.1" />
  <LogicConflationRate value="0.5" />
  <AntiTraceRate value="0.33" />
  <UseAntiDebug value="True"/>

  <!-- Sections -->
  <SectionSettings>
    <Sections targetEnv="darwin_gnu_x86" isProtected="True">
      <Section isProtected="False" name="__objc_classlist"/>
    </Sections>
    <Sections targetEnv="darwin_gnu_arm" isProtected="True">
      <Section isProtected="False" name="__objc_classlist"/>
    </Sections>
    <Sections targetEnv="darwin_gnu_x86_64" isProtected="True">
	  <Section isProtected="False" name="__objc_classlist"/>
    </Sections>
  </SectionSettings>

  <!-- Architecture value -->
  <TargetEnv value="darwin_gnu_arm" />
  <!-- Treat sources from Xcode as absolute paths instead of relative. Sometimes Xcode is in /Applications now -->
  <SDKDir value="/Developer" />
  <SDKDir value="/Applications" />
	
  <Concealer>
    <Enabled value="False"/>
    <Preset value="High"/>
    <High>
    <NumPasses value="10"/>
      <JunkInsertion>
        <Enabled value="True" />
        <OP>
          <Enabled value="True" />
          <SelfVerify value="False" />
          <OperationsPerCondition value="3" />
          <MinIndirections value="1" />
          <MaxIndirections value="5" />
          <MinNodes value="2" />
          <MaxNodes value="5" />
          <MinPointers value="2" />
          <MaxPointers value="5" />
          <MinOperations value="1" />
          <MaxOperations value="5" />
        </OP>
      </JunkInsertion>
      <ConditionedCode>
        <Enabled value="True" />
        <MinBlockSize value="1" />
        <MaxBlockSize value="20" />
        <OP>
          <Enabled value="True" />
          <SelfVerify value="False" />
          <OperationsPerCondition value="3" />
          <MinIndirections value="1" />
          <MaxIndirections value="5" />
          <MinNodes value="2" />
          <MaxNodes value="5" />
          <MinPointers value="2" />
          <MaxPointers value="5" />
          <MinOperations value="1" />
          <MaxOperations value="5" />
        </OP>
        <Mutation>
          <Enabled value="True" />
          <SubstituteLiteral value="True" />
          <SwitchIfElse value="True" />
          <FlipUnaryOperators value="False" />
          <SubstituteBinaryOperators value="False" />
          <SubstituteVariables value="False" />
          <RemoveStatements value="False" />
          <AddJunk value="False" />
        </Mutation>
        </ConditionedCode>
        <IfFlattening>
          <Enabled value="True" />
        </IfFlattening>
        <LoopFlattening>
          <Enabled value="True" />
        </LoopFlattening>
        <LoopUnrolling>
          <Enabled value="True" />
          <MinCount value="1" />
          <MaxCount value="3" />
        </LoopUnrolling>
        <LiteralSubstitution>
          <Enabled value="True" />
        <OP>
          <Enabled value="True" />
          <SelfVerify value="False" />
          <OperationsPerCondition value="3" />
          <MinIndirections value="1" />
          <MaxIndirections value="5" />
          <MinNodes value="2" />
          <MaxNodes value="5" />
          <MinPointers value="2" />
          <MaxPointers value="5" />
          <MinOperations value="1" />
          <MaxOperations value="5" />
        </OP>
        </LiteralSubstitution>
        <StringObfuscation>
          <Enabled value="True" />
        </StringObfuscation>
        <GotoInsertion>
          <Enabled value="True" />
          <MinInsertions value="1" />
          <MaxInsertions value="4" />
        <OP>
          <Enabled value="True" />
          <SelfVerify value="False" />
          <OperationsPerCondition value="3" />
          <MinIndirections value="1" />
          <MaxIndirections value="5" />
          <MinNodes value="2" />
          <MaxNodes value="5" />
          <MinPointers value="2" />
          <MaxPointers value="5" />
          <MinOperations value="1" />
          <MaxOperations value="5" />
        </OP>
        </GotoInsertion>
        </High>
        <Background>
          <NumPasses value="10"/>
        <JunkInsertion>
          <Enabled value="True" />
        <OP>
          <Enabled value="True" />
          <SelfVerify value="False" />
          <OperationsPerCondition value="3" />
          <MinIndirections value="1" />
          <MaxIndirections value="5" />
          <MinNodes value="2" />
          <MaxNodes value="5" />
          <MinPointers value="2" />
          <MaxPointers value="5" />
          <MinOperations value="1" />
          <MaxOperations value="5" />
        </OP>
        </JunkInsertion>
        <ConditionedCode>
          <Enabled value="True" />
          <MinBlockSize value="1" />
          <MaxBlockSize value="20" />
        <OP>
          <Enabled value="True" />
          <SelfVerify value="False" />
          <OperationsPerCondition value="3" />
          <MinIndirections value="1" />
          <MaxIndirections value="5" />
          <MinNodes value="2" />
          <MaxNodes value="5" />
          <MinPointers value="2" />
          <MaxPointers value="5" />
          <MinOperations value="1" />
          <MaxOperations value="5" />
        </OP>
        <Mutation>
          <Enabled value="True" />
          <SubstituteLiteral value="True" />
          <SwitchIfElse value="True" />
          <FlipUnaryOperators value="False" />
          <SubstituteBinaryOperators value="False" />
          <SubstituteVariables value="False" />
          <RemoveStatements value="False" />
          <AddJunk value="False" />
        </Mutation>
        </ConditionedCode>
        <IfFlattening>
          <Enabled value="True" />
        </IfFlattening>
        <LoopFlattening>
          <Enabled value="True" />
        </LoopFlattening>
        <LoopUnrolling>
          <Enabled value="True" />
          <MinCount value="1" />
          <MaxCount value="3" />
        </LoopUnrolling>
        <LiteralSubstitution>
          <Enabled value="True" />
        <OP>
          <Enabled value="True" />
          <SelfVerify value="False" />
          <OperationsPerCondition value="3" />
          <MinIndirections value="1" />
          <MaxIndirections value="5" />
          <MinNodes value="2" />
          <MaxNodes value="5" />
          <MinPointers value="2" />
          <MaxPointers value="5" />
          <MinOperations value="1" />
          <MaxOperations value="5" />
        </OP>
        </LiteralSubstitution>
        <GotoInsertion>
          <Enabled value="True" />
          <MinInsertions value="1" />
          <MaxInsertions value="4" />
        <OP>
          <Enabled value="True" />
          <SelfVerify value="False" />
          <OperationsPerCondition value="3" />
          <MinIndirections value="1" />
          <MaxIndirections value="5" />
          <MinNodes value="2" />
          <MaxNodes value="5" />
          <MinPointers value="2" />
          <MaxPointers value="5" />
          <MinOperations value="1" />
          <MaxOperations value="5" />
        </OP>
      </GotoInsertion>
    </Background>
  </Concealer>
</Config>
