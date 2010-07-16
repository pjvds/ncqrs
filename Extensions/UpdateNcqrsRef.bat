@echo off
REM   This batch file copies the dll of the bin folder of the Ncqrs framework
REM   folder to the lib folder of the MyNotes sample app.
REM   Before executing this script, the Ncqrs solution should be builded. 

IF NOT EXIST "..\Framework\src\Ncqrs\bin\Debug\Ncqrs.dll" GOTO notexists

echo Copying Ncqrs framework to lib...
COPY "..\Framework\src\Ncqrs\bin\Debug\Ncqrs.dll" "lib\Ncqrs"
COPY "..\Framework\src\Ncqrs\bin\Debug\Ncqrs.pdb" "lib\Ncqrs"
echo Done!
GOTO end

:notexists
echo No local Ncqrs found. Please build the NcqrsFramework.sln first.

:end
pause