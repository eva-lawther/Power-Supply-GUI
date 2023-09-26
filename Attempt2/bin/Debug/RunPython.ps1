
$selectscript = $arg[0]

Switch ($selectscript)
{

	'commandClass.py' {python commandClass.py}
	'powerSupplyCommands.py' {python powerSupplyCommands.py}
	'PythonCaller.py' {python PythonCaller.py}
	'sed_json.py' {python sed_json.py}
	'sweeping.py' {python sweeping.py}
    default {
		Write-Output "PowerShell: script not found:"
		Write-Output $selectscript
	}
}
