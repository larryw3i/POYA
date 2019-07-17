#!/bin/bash
$_L="L>_ "
$_ScriptRoot=$PSScriptRoot
$_datetime= (Get-Date -Format lMMddyyyyHHmmss).ToString()
$_SetLocation="Set-Location "+$_ScriptRoot
$_dotnet_ef_migrations_add="dotnet ef migrations add "+$_datetime
$_dotnet_ef_database_update="dotnet ef database update"
$_ok=">>>>\\(^_^)//\\(^_^)//\\(^_^)//\\(^_^)//<<<<"
Write-Output $_L$_SetLocation
Invoke-Expression $_SetLocation
Write-Output $_L$_dotnet_ef_migrations_add
Invoke-Expression $_dotnet_ef_migrations_add
Write-Output $_L$_dotnet_ef_database_update
Invoke-Expression $_dotnet_ef_database_update
Write-Output $_ok
