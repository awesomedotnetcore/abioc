#!/bin/sh -eu

# function to display commands
exe() { echo; echo "\$ $*" ; "$@" ; }

# Parameters
framework="${1-netcoreapp2.1}"
config="${2-Debug}"

testResults="test/TestResults"
include="[abioc]*"
exclude="\"[*.Tests]*,[Abioc.Tests.Internal]*\""

# Cannot use a bash solution in alpine builds https://stackoverflow.com/a/246128
#rootDir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
rootDir=$(pwd)

testProj1="$rootDir/test/Abioc.Tests.Internal/Abioc.Tests.Internal.csproj"
testProj2="$rootDir/test/Abioc.Tests/Abioc.Tests.csproj"

# Restore the packages.
exe dotnet restore "$rootDir"

# Build the test projects
exe dotnet build --no-restore -f "$framework" -c "$config" "$testProj1"
exe dotnet build --no-restore -f "$framework" -c "$config" "$testProj2"

# Execute the tests
exe dotnet test --no-restore --no-build -f "$framework" -c "$config" \
"$testProj1" \
/p:CollectCoverage=true \
/p:Include="$include" \
/p:Exclude="$exclude" \
/p:CoverletOutput="$rootDir/$testResults/internal.coverage.json"

exe dotnet test --no-restore --no-build -f "$framework" -c "$config" \
"$testProj2" \
/p:CollectCoverage=true \
/p:Include="$include" \
/p:Exclude="$exclude" \
/p:MergeWith="$rootDir/$testResults/internal.coverage.json" \
/p:CoverletOutput="$rootDir/$testResults/" \
/p:CoverletOutputFormat="\"json,opencover\""

# Install ReportGenerator if not already installed
if [ ! -f "$rootDir/$testResults/tools/reportgenerator" ]
then
   exe dotnet tool install dotnet-reportgenerator-globaltool --tool-path "$rootDir/$testResults/tools"
fi

# Generate the reports
exe "$rootDir/$testResults/tools/reportgenerator" \
"-verbosity:Info" \
"-reports:$rootDir/$testResults/coverage.opencover.xml" \
"-targetdir:$rootDir/$testResults/Report" \
"-reporttypes:Html"
