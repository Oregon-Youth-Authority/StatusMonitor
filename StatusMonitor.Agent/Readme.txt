This application is used to send diagnostic connectivity data to the JJIS Team.  All data collected is visible here: https://oya-status-monitor.herokuapp.com/.

Please refer to https://www.jjis.oregon.gov/staticcontent/jjisstatusmonitor/faq.html for any questions.

The monitorSettings.json file is not required for this application to run, but can be used to override the computer and source IP.  The value of "Detect" in this file means that the system will try to detect the correct value.  You might want to consider overriding the source IP via configuration if your organization connects to JJIS via a VPN.  You might want to consider overriding the computer name via configuration if there is a value that would be more meaning full to your organization when viewing the data.