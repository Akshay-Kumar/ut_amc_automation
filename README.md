# Torrent Automatic Media Center Tool
A relatively simple windows batch script with a little flavour of C# and filbot to organize all your
torrent downloads. It has the following features:

 * downloads subtitles, posters and fan arts
 * moves the media file as per the media content into either Movies, Music or TV Show directories specified in config file 
 * sends notification through email and also through pushbullet when download finishes
 * supports management of multiple downloaded files simultaneously
 * supports automatic renaming the media files based on movie and TV series episode names from imdb database

## Getting it Setup
After you get a copy of this codebase pulled down locally (either downloaded as a zip or git cloned), you'll need to install the filebot:

    https://get.filebot.net/windows/1

Then you'll need to go into the ./ut_amc_automation_v1.1/config.bat file and update the following values:

 * **AMC_PATH="<optional>"** - Location of the groovy script to be used by the filebot. I recommend to use amc.groovy, if left blank or just amc it picks up the latest groovy source code from the official source online.

* **REPLACE_PATH="<leave this field blank>"** - 
* **CLEANER_PATH="<leave this field blank>"** - 
* **MEDIA=<path>** - Location where you want to store all downloaded torrents.
* **PUSHBULLET_API_TOKEN=<token>** - Pushbullet token.
* **PLEX_URL=<optional>** - If you have installed plex media player and you want to list and organize your torrent media in plex us the plex server url. Default value: localhost.
* **EXCLUDE_LIST=amc-input.txt
* **TV=<tv directory>** - Name of the directory for storing tv series.
* **FILM=<film directory>** - Name of the directory for storing movies.
* **MUSIC=<music directory>** - Name of the directory for storing music files.
* **ANIME=<anime directory>** - Name of the directory for storing animated series.
* **UNSORTED=<Unsorted>
* **SERIES_FORMAT={n}/Season {s.pad(2)}/{n} - {s00e00} - {t}** - Use as it is.
* **ANIME_FORMAT={n} - {s00e00} - {t}** - Use as it is
* **MOVIE_FORMAT={n} ({y})/{fn}** - Us as it is
* **MUSIC_FORMAT={n}/{album+'/'}{pi.pad(2)+'. '}{artist} - {t}** - Use as it is
* **CURRENT_DIRECTORY=<directory>** - Location where you have placed ut_amc_automation.exe
* **UNSORTED_FORMAT={file.structurePathTail}
* **USER_NAME=<email from>** -Your email address 
* **PASSWORD=<password token>** - Password token get it from google apps.
* **EMAIL_TO=<email to>** - Email to send notification to.

Once you've updated all of your parameter information, you'll need to configure the following in your bit torrent application or utorrent or any other torrent client to finalize your setup:

* Open bit torrent and navigate to options->preferences->Run Program(AT THE BOTTOM)
* Paste the following command in the text box that says "Run this program when a torrent finishes:"

	<fill path of the location of the executable file>\ut_amc_automation.exe "<full path of the excutable file>\ut_amc_automation.bat" "%D" "%F" "%K" "%N" "%L" "%S" "%I"

The parameters passed above and their meaning are as following:

You can use the following parameters:

 * %F - Name of downloaded file (for single file torrents)
 * %D - Directory where files are saved
 * %N - Title of torrent
 * %P - Previous state of torrent
 * %L - Label
 * %T - Tracker
 * %M - Status message string (same as status column)
 * %I - hex encoded info-hash
 * %S - State of torrent
 * %K - kind of torrent (single|multi)

Where State is one of:

 * Error - 1
 * Checked - 2
 * Paused - 3
 * Super seeding - 4
 * Seeding - 5
 * Downloading - 6
 * Super seed [F] - 7
 * Seeding [F] - 8
 * Downloading [F] - 9
 * Queued seed - 10
 * Finished - 11
 * Queued - 12
 * Stopped - 13
 * Queued - 12
 * Preallocating - 17
 * Downloading Metadata - 18
 * Connecting to Peers - 19
 * Moving - 20
 * Flushing - 21
 * Need DHT - 22
 * Finding Peers - 23
 * Resolving - 24
 * Writing - 25
 
## How it Works
As soon as you torrent finishes the download it runs the ut_amc_automation and the application accepts the parameters from the torrent client and perform the specific task.
