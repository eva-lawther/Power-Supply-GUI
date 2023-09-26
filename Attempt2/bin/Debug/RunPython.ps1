
$selectscript = $args[0]
Switch ($selectscript)
{

	'channelClass.py' {python channelClass.py}
	'commandClass.py' {python commandClass.py}
	'module1.py' {python module1.py}
	'powerSupplyCommands.py' {python powerSupplyCommands.py}
	'PythonCaller.py' {python PythonCaller.py}
	'sed_json.py' {python sed_json.py}
	'sweeping.py' {python sweeping.py}
    default {
		Write-Output "PowerShell: script not found:"
		Write-Output $selectscript
	}
}
