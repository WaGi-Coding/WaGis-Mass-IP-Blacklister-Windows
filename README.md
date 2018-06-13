# WaGi's IP-Blacklister (Windows Firewall)
<<<<<<< HEAD
[>Executable Download<](https://ip-blacklister.wagi-coding.com/IP-Blacklister-v1-2-9.zip "WaGi-Coding")  
=======
[>DOWNLOAD HERE<](https://github.com/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases)  
>>>>>>> 0f6fa98e564b2c41ed2bd6318f0add4fae2f748f
![Application Overview](https://i.imgur.com/uBFNba4.png)



## About:

__Why you made this?__

I had to automatically block a mass of IP-Addresses. I did it with a Batch script first, but hell that was taking about 2 hours every time for 20k IPs.

__What is this Application for?__

* Blocking a mass of IPs through the Windows Firewall
* Block IPs from a List
* Interval based Blocking IPs from List
* Being faster(5 seconds instead of 2h with batch script)

__How it works?__

* It splits the IPs from the left Textbox into 5k lists and makes 1 Firewall Rule per 5k IPs.

__How does the IP-List needs to look like?__

* There is no need for special formatting.
* The Programm will grab every IPv4 found in a List.
* Just make sure they are seperated somehow

__Additional Info__
* This programm does not change/delte any Firewall-Rules not made by itself.
* There are no unique checks for the IPs!
* If you click the DEL-Button, you only delete the rules matching the selected Protocol & Direction.
* Automatic-Mode will always delete the existing rules matching the selected Protocol & Direction.
* This programm was mostly made to update the Firewall from IP-Blacklists.

__.NET Framework 4.6.1__

__Tested on:__
* Windows 10
* Windows Server 2012 R2
* Windows Server 2016
