echo off
echo ###########################################################################################################################
echo						Script for removing finished torrent file
echo ###########################################################################################################################
rem call C:\Users\AkshayKumar\Desktop\Torrent_Work\ut_config\config.bat "%~1" "%~2" "%~3" "%~4" "%~5" "%~6" "%~7"
echo ****************************************************************************************************************************
echo Running script for removing finished torrent file : "%ut_title%"
echo ****************************************************************************************************************************
rem if %ut_state%==11 GOTO CONTINUE
GOTO FINISH
GOTO CONTINUE
GOTO:EOF
rem Deleting Torrents Script
:CONTINUE
echo ****************************************************************************************************************************
echo Entering CONTINUE block ::  File Name : "%ut_title%"
echo ****************************************************************************************************************************
FINDSTR /L /C:"%ut_title%" "%MEDIA%"\amc-input.txt
echo checking if processing of media files is over
if %ERRORLEVEL%==0 echo processing of files is over. Now starting cleanup...
if %ERRORLEVEL%==1 echo processing of files still in progress. Exiting...
if %ERRORLEVEL%==0 goto REMOVE_TORRENT 
if %ERRORLEVEL%==1 goto EXIT
GOTO:EOF

:REMOVE_TORRENT 
rem Parameter usage: basedir torrent-name state kind [filename]
rem corresponds to uTorrents flags: %D %N %S %K %F %L
echo ****************************************************************************************************************************
echo Entering REMOVE_TORRENT block ::  File Name : "%ut_title%"
echo ****************************************************************************************************************************
echo Run on %date% at %time%
echo Input: %ut_dir% %ut_title% %ut_state% %ut_kind% %ut_file% %ut_label%
echo %date% at %time%: Handling torrent :: %ut_title%
call powershell -executionpolicy unrestricted -file %CURRENT_DIRECTORY%\Remove-Torrent.ps1 %ut_info_hash% %ut_state%
if %ERRORLEVEL%==0 echo Torrent removed successfully from bit-torrent client. Success...
if %ERRORLEVEL%==1 echo Error occurred while removing torrent from bit-torrent client. Error...
if %ERRORLEVEL%==0 GOTO DELETING
if %ERRORLEVEL%==1 GOTO ERROR
GOTO:EOF

:DELETING
echo **Deleting
	if "%ut_kind%"=="single" GOTO DELETE_FILE
	if "%ut_kind%"=="multi" GOTO DELETE_ALL
	echo ERROR - Unrecognized kind (%ut_kind%)
GOTO:EOF

:DELETE_FILE
echo ****************************************************************************************************************************
echo Entering DELETE_FILE block ::  File Name : "%ut_title%"
echo ****************************************************************************************************************************
echo Deleting file %ut_dir%\%ut_file%
del "%ut_dir%"\"%ut_file%"
if %ERRORLEVEL%==0 echo "%ut_file%" deleted successfully.
if %ERRORLEVEL%==1 echo Error occurred while deleting files/directory.
if %ERRORLEVEL%==1 GOTO ERROR
if %ERRORLEVEL%==0 GOTO FINISH
GOTO:EOF

:DELETE_ALL
rem Simply some precautions here
echo ****************************************************************************************************************************
echo Entering DELETE_ALL block ::  File Name : "%ut_title%"
echo ****************************************************************************************************************************
if "%ut_dir%"=="G:\Media1" goto EXIT
if "%ut_dir%"=="G:\Music" goto EXIT
if "%ut_dir%"=="G:\Porn" goto EXIT
if "%ut_dir%"=="C:\" goto EXIT
echo Deleting directory %ut_dir% with all subcontent
rmdir /S /Q "%ut_dir%"
if %ERRORLEVEL%==0 echo "%ut_dir%" deleted successfully.
if %ERRORLEVEL%==1 echo Error occurred while deleting files/directory.
if %ERRORLEVEL%==1 GOTO ERROR
if %ERRORLEVEL%==0 GOTO FINISH
GOTO:EOF

:ERROR
echo ****************************************************************************************************************************
echo Entering ERROR block ::
echo ****************************************************************************************************************************
echo ERRORLEVEL : {%ERRORLEVEL%} some error occurred while executing script view deleted.log for details.
call powershell -executionpolicy unrestricted -file %CURRENT_DIRECTORY%\send-email.ps1 "%USER_NAME%" "%EMAIL_TO%" "%PASSWORD%" "%CURRENT_DIRECTORY%" "%PUSHBULLET_API_TOKEN%" "%ut_title%" "Error occurred while cleaning %ut_title%" "ERROR"
if %ERRORLEVEL%==1 echo Error occurred while sending email.
if %ERRORLEVEL%==0 echo Email sent successfully.
if %ERRORLEVEL%==1 GOTO EXIT
echo ****************************************************************************************************************************
echo Exiting ERROR block ::
echo ****************************************************************************************************************************
GOTO:EOF

:EXIT
echo exiting script file.
exit /b 0

:FINISH
echo ****************************************************************************************************************************
echo Entering FINISH block ::
echo ****************************************************************************************************************************
call powershell -executionpolicy unrestricted -file %CURRENT_DIRECTORY%\send-email.ps1 "%USER_NAME%" "%EMAIL_TO%" "%PASSWORD%" "%CURRENT_DIRECTORY%" "%PUSHBULLET_API_TOKEN%" "%ut_title%" "FileBot finished cleaning %ut_title%" "SUCCESS"
if %ERRORLEVEL%==1 echo Error occurred while sending email.
if %ERRORLEVEL%==0 echo Email sent successfully.
if %ERRORLEVEL%==1 GOTO EXIT
echo ****************************************************************************************************************************
echo Exiting FINISH block ::
echo ****************************************************************************************************************************
exit /b 0