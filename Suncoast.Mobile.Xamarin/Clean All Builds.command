#!/bin/bash
cd "$(dirname "$0")"
pwd
ls

find . -name obj -exec rm -rf {} \;
find . -name bin -exec rm -rf {} \;

osascript -e 'tell application "Terminal" to quit' &
exit
