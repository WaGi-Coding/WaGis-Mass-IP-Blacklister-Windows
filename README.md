# WaGi Coding's IP-Blacklister (Windows Firewall)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows?label=Latest%20Version&style=for-the-badge)

![GitHub all releases](https://img.shields.io/github/downloads/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/total?label=Github%20Release%20Downloads)
![SourceForge](https://img.shields.io/sourceforge/dt/wagi-ip-blacklister?label=SourceForge%20Downloads)

[>DOWNLOAD HERE<](https://github.com/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases)  
![Application Overview](https://i.imgur.com/Pn7tANy.png)



## About:

__Why you made this?__

I had to automatically block a mass of IP-Addresses. I did it with a simple Batch script first, but hell that was taking about 2 hours every time for 20k IPs, so i decided to create this app after realizing the creating of rules is much much faster that way.

__What is this Application for?__

* Blocking a mass of IPs through the Windows Firewall
* Block IPs from a List
* Interval based Blocking IPs from List
* Being faster(5 seconds instead of 2h with batch script)

__How it works?__

* It splits the IPs from the left Textbox into 10k lists and makes 1 Firewall Rule per 10k IPs. (Depends on the OS Version)

__How does the IP-List needs to look like?__

* There is no need for special formatting.
* The Program will grab every IPv4 & IPv6(>= v1.3.5 + IPv4 & IPv6 CIDR/Range support) found in a List.
* Just make sure they are seperated somehow

__Additional Info__
* This program does not change/delte any Firewall-Rules not made by itself.
* There are no unique checks for the IPs!
* If you click the DEL-Button, you only delete the rules matching the selected Protocol & Direction.
* Automatic-Mode will always delete the existing rules matching the selected Protocol & Direction.
* This program was mostly made to update the Firewall from IP-Blacklists.

__.NET Framework 4.6.1__

__Tested on:__
* Windows 10 Pro
* Windows Server 2012 R2
* Windows Server 2008 R2
* Windows Server 2016
* Windows 7 (1k Block Size instead of 10k - Program now does this automatically)
