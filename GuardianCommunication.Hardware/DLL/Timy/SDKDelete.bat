cd /d %~dp0
if /i "%PROCESSOR_IDENTIFIER:~0,3%"=="X86" (
		echo system is x86
		regsvr32 %windir%\system32\FP_CLOCK.ocx -u
		regsvr32 %windir%\system32\FPCLOCK_Svr.ocx -u
		del %windir%\system32\FP_CLOCK.ocx
		del %windir%\system32\FPCLOCK_Svr.ocx
		del %windir%\system32\FP_CLOCK.oca
		del %windir%\system32\TMPCCOMM.dll
	) else (
		echo system is x64
		regsvr32 %windir%\SysWOW64\FP_CLOCK.ocx -u
		regsvr32 %windir%\SysWOW64\FPCLOCK_Svr.ocx -u
		del %windir%\SysWOW64\FP_CLOCK.ocx
		del %windir%\SysWOW64\FPCLOCK_Svr.ocx
		del %windir%\SysWOW64\FP_CLOCK.oca
		del %windir%\SysWOW64\TMPCCOMM.dll
	)
pause