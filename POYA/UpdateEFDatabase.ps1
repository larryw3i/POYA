$_PSScriptRoot=$PSScriptRoot
$_L="L>_ "
Set-Location $_PSScriptRoot
$_L+$_PSScriptRoot
$_datetime= (Get-Date -Format lMMddyyyyHHmmss)
$_datetime=$_datetime.ToString()
$_L+$_datetime
dotnet ef migrations add $_datetime
$_L+"dotnet ef migrations add "+$_datetime
dotnet ef database update
$_L+"dotnet ef database update"
">>>>(^_^)(^_^)(^_^)(^_^)<<<<"