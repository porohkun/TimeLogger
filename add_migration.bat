@echo off
cd TimeLogger
dotnet ef migrations add %1 --startup-project . --project ../TimeLogger.DataAccess