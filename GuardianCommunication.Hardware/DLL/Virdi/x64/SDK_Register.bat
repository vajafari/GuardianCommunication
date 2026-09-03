cd /d %~dp0
if /i "%PROCESSOR_IDENTIFIER:~0,3%"=="X86" (
	echo "system is x86 and DLLs cannot be registered"
	) else (
		
		copy .\*.dll %windir%\system32\
		regsvr32 /s /c %windir%\system32\UCSAPICOM.dll
	)
