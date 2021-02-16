# Tabl_

**Tabl_** is a spreadsheet interface within Rhino for the viewing, editing, and exporting of object properties. Objects are linked to the spreadsheet in real-time, allowing Tabl_ to automatically update as objects are modified.

Comments or ideas welcome at HotCoffeeGroup@gmail.com :e-mail:

Feel free to report bugs by opening an issue in this repo.

## Requirements
Latest version requires Rhino 6 or Rhino 7 running on Windows.

## Installation
1. Download the latest release from https://www.food4rhino.com/app/tabl

2. Run the `.rhi` file and follow the instructions. 

3. Type 'LaunchTabl_" into the Rhino command line to view that dockable tab.


## Uninstallation
To remove Tabl_ on a Windows machine, close Rhino first. Then go to "%AppData%\McNeel\Rhinoceros\7.0\Plug-ins\" (replace 7.0 with corresponding Rhino version) and delete the **Tabl_** folder. Restart Rhino and it will be uninstalled. 

## Versions

0.x versions are cross-platform. **Tabl_** can run on Windows and MacOS machines.

2.x versions are for Windows machine only.

## Features

* "Live" Spreadsheet interface running while modeling in Rhino.
* Analyze and display multiple object properties simultaneously.
* Live update as objects are modified.
* Display an extensive list of object properties.
* Simple export to spreadsheet friendly formats.
* Sort, count, and total object properties.
* Flexible formatting masks on data points.
* Place the **Tabl_** into the Rhino document as text objects.

**Tabl_** provides a column and row spreadsheet in which objects can be added and removed from. The user can then check which of the nearly two dozen properties to display as the columns in the spreadsheet. Each column can be sorted to provide a unique way of examining object properties. **Tabl_** can also provide totals for numeric properties, allowing instant feedback on how the geometry is meeting project goals. Additionally, multiple options of formatting are available for numbers and colors, depending on the user requirements.

Once finished organizing and formatting the data, **Tabl_** can export information to a CSV, or simply copy to the clipboard for pasting into other spreadsheet programs. **Tabl_** can also be placed into the Rhino document model space or layout space as editable text objects.

## Example Uses

* Architecture schedules.
* Area take-offs.
* Selecting objects based on object property.
* Examining large amounts of geometry.
* Extracting object information.

## Starter Guide

Type "LaunchTabl" in Rhino to call up the main interface. From v2.0 onwards, **Tabl_** integrates with Rhino docked panels so it can be snapped and sorted with other tabbed tools.

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Add%402x.png) click to select objects in the Rhino model and add to **Tabl_**

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Export%402x.png) export full **Tabl_** spreadsheet as a CSV document

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Import%402x.png) import from CSV files (must have GUID column for **Tabl_** to recognize Rhino objects)

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Info%402x.png) About splash

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Placement%402x.png) place a **Tabl_** in the Rhino 3d environment

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Refresh%402x.png) refresh **Tabl_**

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Remove%402x.png) click to remove from Rhino document by selecting them in the model

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Select%402x.png) select what's highlighted in **Tabl_** in the Rhino model

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Trash%402x.png) delete all records in **Tabl_** (does not delete Rhino object)

![alt text](https://github.com/tmwarchitecture/Tabl_/blob/master/cs/2.0/Resources/Settings%402x.png) settings for **Tabl_**

Right click in the main **Tabl_** area to access context menu options, one of which is to select the object properties to display (spreadsheet columns). 

Press **Delete** to remove a highlighted element from Tabl_.
