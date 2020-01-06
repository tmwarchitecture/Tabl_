# Tabl_

Tabl_ is a spreadsheet interface within Rhino for the viewing, editing, and exporting of object properties. Objects are linked to the spreadsheet in real-time, allowing Tabl_ to automatically update as objects are modified.

## Features

* ‘Live’ Spreadsheet interface running while modeling in Rhino.
* Analyze and display multiple object properties simultaneously.
* Live update as objects are modified.
* Display up to 23 different object properties.
* Simple export to TXT,  CSV, and HTML Tables.
* Sort, count, and total object properties.
* Color and number formatting options.
* Place the Tabl_ into the Rhino document as text objects. (NEW IN 0.2) 

Tabl_ provides a column and row spreadsheet in which objects can be added and removed from. The user can then check which of the 23 properties to display as the columns in the spreadsheet. Each column can be sorted to provide a unique way of examining object properties. Tabl_ can also provide totals for certain columns, allowing instant feedback on how the geometry is meeting project goals. Additionally, multiple options of formatting are available for numbers and colors, depending on the user requirements.

Once finished organizing and formatting the data, Tabl_ can export the spreadsheet to HTML, CSV, or TXT files or simply copy to the clipboard for pasting into other spreadsheet programs.

As of version 0.2, the Tabl_ can then be placed into the Rhino document model space or layout space as editable text objects.

## Version 0.2 Changes

* Place Tabl: Places the Tabl into the Rhino document.
* All parameters sortable
* Export and Import GUIDs
* Custom Units: Custom unit name and scale
* New parameters:
  * CenterX: X coordinate of an objects’ bounding box center point.
  * CenterY: Y coordinate of an objects’ bounding box center point.
  * NumFaces: Counts the number of faces on a polysurface or mesh.
  * Comments: Saves to each objects “User Text” as “tabl.comment”


## Example Uses

* Architecture schedules.
* Area take-offs.
* Selecting objects based on object property.
* Examining large amounts of geometry.
* Extracting object information.
* Further information and instructions can be found in the User Manual.

Works with Rhino 6 ONLY!!!

Comments, Issues, and ideas welcome at HotCoffeeGroup@gmail.com
