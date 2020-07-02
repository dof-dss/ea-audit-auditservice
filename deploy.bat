@echo off

rem This batch file builds application and deploys to GOV UK PaaS
rem Please supply environment (sandbox, staging or production) followed by PaaS username and password

set environment=%1
set username=%2

call dotnet build -c %environment%
call cf login -u %username% -o dof-dss -s %environment%

cd .\EA.Audit.AuditService\bin\%environment%\netcoreapp3.1\publish
call cf push

cd ..\..\..\..\..\EA.Audit.SubscriberApi\bin\%environment%\netcoreapp3.1\publish
call cf push
