# V-derdataProjectAppCore


Project work - Weather data

* Initially execute DatabaseSetup and change the connection string accordingly.

 The project is an application that, based on existing
temperature and humidity data can search, sort
and draw conclusions.

◦ In this project, we submit solutions individually.
◦ Working together is always allowed
◦ Data file in text format, comma separated.
◦ The data file is authentic, and has data errors, and gaps

◦ Exempel på data:
https://docs.google.com/spreadsheets/d/1CmOA9nZnohgPZ5e9HpKQRNF5koWARg59
JYAPS6P2r-M/edit#gid=0


It must be possible to display the following information
Outside:
◦ Average temperature for selected date (search option)
◦ Sorting of warmest to coldest day according to average temperature per day
◦ Sorting the dryest to the wettest day according to average humidity per day
◦ Sorting of the least to greatest risk of mold
◦ Date of meteorological Autumn
◦ Date for Meteorological Winter (NOTE! The winter of 2016 was mild)

Indoor
◦ Average temperature for selected date (search option)
◦ Sorting of warmest to coldest day according to average temperature per day
◦ Sorting the dryest to the wettest day according to average humidity per day
◦ Sorting of the least to greatest risk of mold

Issues to be resolved
◦ Selection of data types in DB for this type of data.
◦ Loading text file into the database
◦ Algorithms that calculate aggregate data
◦ Meteorological rules for Autumn, Winter and Mold Index
◦ Mold risk, find formula.

Other requirements
◦ The application uses .NET 5 (latest version)
◦ The source code is documented in the current code, with special emphasis on the use of
algorithms. Explain your choices of algorithms and data structures.
◦ Data access must be done with Entity Framework (Core)
◦ Feel free to compare the results you get in your app with other students, to share with you how
you thought.
◦ All requirements and specifications can be changed during the work

VG information
To get VG, the program must also be able to make assessments on the following issues:
◦ How long is the balcony door open per day, and sorted on when it is the longest
open.
◦ The assumption is that if the balcony door is opened, the internal temperature drops slightly.
◦ The outside temperature is also raised slightly, as the thermometer is close to the balcony door.
◦ Sorting when the indoor and outdoor temperatures differ most and least.
