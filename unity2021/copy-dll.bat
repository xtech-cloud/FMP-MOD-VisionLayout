
@echo off

REM !!! Generated by the fmp-cli 1.70.0.  DO NOT EDIT!

md VisionLayout\Assets\3rd\fmp-xtc-visionlayout

cd ..\vs2022
dotnet build -c Release

copy fmp-xtc-visionlayout-lib-mvcs\bin\Release\netstandard2.1\*.dll ..\unity2021\VisionLayout\Assets\3rd\fmp-xtc-visionlayout\
