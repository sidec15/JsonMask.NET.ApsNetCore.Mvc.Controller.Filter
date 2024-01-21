#!/bin/bash

set -e # Exit on error

# configuration
project=JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Tests
solution="JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.sln"
coverage_report_title=JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
report_assembly_filters="+JsonMask.NET.ApsNetCore.Mvc.Controller.Filter"
# report_assembly_filters="-ExternalServices"
# report_class_filters="-Repository.Migrations*"

usage="USAGE "$0" [-h] [OPTION] 
-- Run tests, create the code coverage

where:
    -h, --help                    Show this help text
    
"

function readCommandLine() {

  while [ $# -ge 1 ]; do
    key="$1"
    case $key in
    -h | --help)
      echo "$usage"
      exit 0
      ;;
    *)
      # unknown option
      ;;
    esac
    shift # past argument or value
  done

}

function runCoverage() {

  local coverage_base_dir="$project/TestResults/CodeCoverage"
  if [ -e "$coverage_base_dir" ]; then
    rm -rf "$coverage_base_dir"
  fi

  dotnet build $solution
  # dotnet test --settings $project/coverlet.runsettings
  dotnet test --settings $project/code-coverage.runsettings

  # get the most recent created directory
  local dir_name=$(\ls -t $coverage_base_dir | head -n 1)
  local coverage_output_dir="$coverage_base_dir/$dir_name"

  for f in "$coverage_output_dir"/*; do
    # if [[ "$f" == *opencover.xml ]]; then
    #   cp $f $coverage_base_dir/coverage-opencover.xml
    # fi
    if [[ "$f" == *cobertura.xml ]]; then
      cp $f $coverage_base_dir/coverage-cobertura.xml
    fi    
  done

  reportgenerator -reports:"$coverage_base_dir/coverage-cobertura.xml" -targetdir:"CodeCoverage/output/report" -reporttypes:"TextSummary;Html" -title:$coverage_report_title -assemblyfilters:"$report_assembly_filters" -classfilters:"$report_class_filters"
  # reportgenerator -reports:"$coverage_base_dir/coverage-opencover.xml" -targetdir:"CodeCoverage/output/report" -reporttypes:"TextSummary;Html" -title:$coverage_report_title -assemblyfilters:"$report_assembly_filters" -classfilters:"$report_class_filters"

}

readCommandLine "$@"
runCoverage
