#!/bin/bash
_L="L>_"
_ScriptRoot=$(cd "$(dirname "$0")";pwd)
_DateTime=`date +%m%d%Y%H%M%S`
_cd="cd "$_ScriptRoot
_dotnet_ef_migrations_add="dotnet ef migrations add "$_DateTime
_dotnet_ef_database_update="dotnet ef database update"
_ok=">>>>\\(^_^)//\\(^_^)//\\(^_^)//\\(^_^)//<<<<"
echo	$_L$_cd
		${_cd}
echo	$_L$_dotnet_ef_migrations_add
		${_dotnet_ef_migrations_add}
echo	$_L$_dotnet_ef_database_update
		${_dotnet_ef_database_update}
echo	$_L$_ok
