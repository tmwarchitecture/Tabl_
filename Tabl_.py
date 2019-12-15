import Rhino
import Rhino.UI
import Eto.Drawing as drawing
import Eto.Forms as forms
import rhinoscriptsyntax as rs
import scriptcontext as sc
import os.path as op

class SettingsDialog(forms.Dialog):
    def __init__(self):
        self.Title = "Settings"
        self.Padding = drawing.Padding(5)
        self.Resizable = False
        self.CreateLayout()
        path = Rhino.PlugIns.PlugIn.PathFromName("Tabl_")
        dirName = op.dirname(path)
        iconPath = dirName + r"\mainIcon.ico"
        self.Icon = drawing.Icon(iconPath)
        #Variables
        self.applyBool = False

    def CreateLayout(self):
        self.CreateControls()
        self.colorFormatGroup = forms.GroupBox(Text = "Color Format")
        self.colorFormatLayout = forms.DynamicLayout()
        self.colorFormatLayout.AddSeparateRow(self.colorRadioBtn, None)
        self.colorFormatGroup.Content = self.colorFormatLayout

        self.numFormatGroup = forms.GroupBox(Text = "Number Format")
        self.numFormatLayout = forms.DynamicLayout()
        self.numFormatLayout.AddSeparateRow(self.label1, self.numericDecPlaces, None)
        self.numFormatLayout.AddSeparateRow(self.showCommas, None)
        self.numFormatGroup.Content = self.numFormatLayout

        layout = forms.DynamicLayout()
        layout.AddSeparateRow(self.numFormatGroup, None)
        layout.AddSeparateRow(self.colorFormatGroup)
        layout.AddSeparateRow(self.lblVersion, None)
        layout.AddSeparateRow(None, self.btnCancel, self.btnApply)
        layout.AddRow(None)

        self.Content = layout

    def CreateControls(self):
        self.label1 = forms.Label()
        self.label1.Text = "Decimal Places"
        
        self.lblVersion = forms.Label(Text = version)

        self.showCommas = forms.CheckBox(Text = "Thousands Separator")
        try:
            self.showCommas.Checked = commaSep
        except:
            print "showCommas failed"

        self.btnApply = forms.Button()
        self.btnApply.Text = "OK"
        self.btnApply.Click += self.OnApplySettings

        self.btnCancel = forms.Button()
        self.btnCancel.Text = "Cancel"
        self.btnCancel.Click += self.OnCancelSettings

        self.numericDecPlaces = forms.NumericUpDown()
        self.numericDecPlaces.DecimalPlaces  = 0
        self.numericDecPlaces.MaxValue = 8
        self.numericDecPlaces.MinValue = 0
        
        try:
            self.numericDecPlaces.Value = int(decPlaces)
        except:
            print "numericDecPlaces failed"

        self.colorRadioBtn = forms.RadioButtonList()
        self.colorRadioBtn.DataStore = ["Name", "R-G-B", "R,G,B"]
        self.colorRadioBtn.Orientation = forms.Orientation.Vertical
        try:
            self.colorRadioBtn.SelectedIndex = colorFormat
        except:
            print "colorRadioBtn failed"

    def OnApplySettings(self, sender, e):
        try:
            global decPlaces
            decPlaces = self.numericDecPlaces.Value
            global commaSep
            commaSep = self.showCommas.Checked
            global colorFormat
            colorFormat = self.colorRadioBtn.SelectedIndex
            sc.sticky['decPlaces'] = int(decPlaces)
            sc.sticky['commaSep'] = bool(commaSep)
            sc.sticky['colorFormat'] = int(colorFormat)
            self.applyBool = True
            self.Close()
        except:
            print "Failed to apply settings"

    def OnCancelSettings(self, sender, e):
        try:
            self.Close()
        except:
            print "Failed to cancel settings"

class Tabl_Form(forms.Form):
    def __init__(self):
        ########################################################################
        #Setup Form
        self.Initialize()
        #Create Controls
        self.CreateLayout()
        #Check Sticky
        self.CheckSticky()
        #Setup Grid
        self.Regen()
        #Events
        self.CreateEvents()

    #Initialize
    def Initialize(self):
        ########################################################################
        #Form Setup
        self.Title = "Tabl_"
        self.Padding = 10
        self.Resizable = True
        if 'dialogPos' in sc.sticky:
            self.Location = sc.sticky['dialogPos']
        
        if 'dialogSize' in sc.sticky:
            self.Size = sc.sticky['dialogSize']
        else:
            self.Size = drawing.Size(600, 600)
        self.MinimumSize = drawing.Size(600, 600)
        self.Closed += self.closeDialog
        self.SizeChanged += self.OnSizeChanged
        
        

        ########################################################################
        #Form Icon
        def loadImages():
            path = Rhino.PlugIns.PlugIn.PathFromName("Tabl_")
            dirName = op.dirname(path)
            try:
                iconPath = dirName + r"\mainIcon.ico"
                self.Icon = drawing.Icon(iconPath)
            except:
                print "Could not load mainIcon.ico"

            try:
                addIconPath = dirName + r"\addIcon.ico"
                self.addImage = drawing.Icon(addIconPath)
            except:
                self.addImage = None
                print "Could not load addIcon.ico"

            try:
                removeIconPath = dirName + r"\removeIcon.ico"
                self.removeImage = drawing.Icon(removeIconPath)
            except:
                self.removeImage = None
                print "Could not load removeIcon.ico"

            try:
                updateIconPath = dirName + r"\refreshIcon.ico"
                self.updateImage = drawing.Icon(updateIconPath)
            except:
                self.updateImage = None
                print "Could not load refreshIcon.ico"
        loadImages()

    #Create Layout
    def CreateLayout(self):
        #Create Grid, Buttons, Checkboxes, Dropdowns
        self.CreateControls()
        ########################################################################
        #Layout
        try:
            layout = forms.DynamicLayout()
            layout.Spacing = drawing.Size(5, 5)
            layout.AddSeparateRow(self.paramPanel, self.gridGroupBox)
            layout.AddSeparateRow(None)
            self.Content = layout
        except:
            print "CreateLayout() failed"

    def CreateControls(self):
        try:
            ########################################################################
            #GridView - Spreadsheet
            def createGrid():
                self.grid = forms.GridView()
                self.grid.BackgroundColor = drawing.Colors.LightGrey
                self.grid.AllowColumnReordering = False
                self.grid.GridLines = forms.GridLines.Both
                self.grid.Border = forms.BorderType.Line
                self.grid.AllowMultipleSelection = True
                self.grid.Size = drawing.Size(700,500)
                self.grid.CellEdited += self.OnNameCellChanged
                self.grid.ColumnHeaderClick += self.sortColumn
            createGrid()
            ########################################################################
            #Context menu
            def createContextMenu():
                self.changeSelectionName = forms.ButtonMenuItem()
                self.changeSelectionName.Text = "Change Selected Object(s) Name"
                self.changeSelectionName.Click += self.OnNameChanged

                self.changeSelectionLayer = forms.ButtonMenuItem()
                self.changeSelectionLayer.Text = "Change Selected Object(s) Layer"
                self.changeSelectionLayer.Click += self.OnLayerChanged

                self.changeSelectionColor = forms.ButtonMenuItem()
                self.changeSelectionColor.Text = "Change Selected Object(s) Color"
                self.changeSelectionColor.Click += self.OnColorChanged

                self.addItem = forms.ButtonMenuItem()
                self.addItem.Text = "Add Objects"
                try:
                    self.addItem.Image = self.addImage
                except:
                    pass
                self.addItem.Click += self.AddByPicking

                self.removeItem = forms.ButtonMenuItem()
                self.removeItem.Text = "Remove Objects"
                try:
                    self.removeItem.Image = self.removeImage
                except:
                    pass
                self.removeItem.Click += self.RemoveSelection

                self.selectItem = forms.ButtonMenuItem()
                self.selectItem.Text = "Select Objects"
                self.selectItem.Click += self.OnSelectFromGrid

                self.seperator1 = forms.SeparatorMenuItem()
                self.seperator2 = forms.SeparatorMenuItem()
                self.seperator3 = forms.SeparatorMenuItem()

                self.copyItems = forms.ButtonMenuItem()
                self.copyItems.Text = "Copy All"
                self.copyItems.Click += self.copyToClipboard

                self.copySelection = forms.ButtonMenuItem()
                self.copySelection.Text = "Copy Selection"
                self.copySelection.Click += self.copySelectionToClipboard

                self.contextMenu = forms.ContextMenu([self.changeSelectionName, self.changeSelectionLayer, self.changeSelectionColor, self.seperator3, self.addItem, self.removeItem, self.seperator1, self.selectItem, self.seperator2, self.copyItems, self.copySelection])
                self.grid.ContextMenu = self.contextMenu
            createContextMenu()
            ########################################################################
            #Radiobutton - Auto-manual
            self.radioMode = forms.RadioButtonList()
            self.radioMode.DataStore = ["Automatic", "Manual"]
            self.radioMode.Orientation = forms.Orientation.Vertical
            try:
                self.radioMode.SelectedIndex = sc.sticky['radioMode']
            except:
                self.radioMode.SelectedIndex = 1
            self.radioMode.SelectedIndexChanged += self.OnRadioChanged
            ########################################################################
            self.countLbl = forms.Label(Text = "Selection Error")
            ########################################################################
            #Checkboxes & Columns
            def createCheckboxes():
                #Checkbox - Show Number
                self.showNum = forms.CheckBox()
                self.showGUID = forms.CheckBox()
                self.showType = forms.CheckBox()
                self.showName = forms.CheckBox()
                self.showLayer = forms.CheckBox()
                self.showColor = forms.CheckBox()
                self.showLinetype = forms.CheckBox()
                self.showP_Color = forms.CheckBox()
                self.showP_Width = forms.CheckBox()
                self.showMaterial = forms.CheckBox()
                self.showLength = forms.CheckBox()
                self.showArea = forms.CheckBox()
                self.showVol = forms.CheckBox()
                self.showNumEdges = forms.CheckBox()
                self.showNumPts = forms.CheckBox()
                self.showDegree = forms.CheckBox()
                self.showCenterZ = forms.CheckBox()
                self.showIsPlanar = forms.CheckBox()
                self.showIsClosed = forms.CheckBox()
                
                

                #Checkbox - Show Units
                self.showUnits = forms.CheckBox()
                if 'showUnitsChecked' in sc.sticky:
                    self.showUnits.Checked = sc.sticky['showUnitsChecked']
                else:
                    self.showUnits.Checked = False
                self.showUnits.Text = "Show Units\t"
                self.showUnits.CheckedChanged += self.showUnitsChanged

                #Checkbox - Show Total
                self.showTotal = forms.CheckBox()
                if 'showTotalChecked' in sc.sticky:
                    self.showTotal.Checked = sc.sticky['showTotalChecked']
                else:
                    self.showTotal.Checked = False
                self.showTotal.Text = "Show Total\t"
                self.showTotal.CheckedChanged += self.showTotalChanged

                #Checkbox - Show Headers
                self.showHeaders = forms.CheckBox()
                if 'showHeadersChecked' in sc.sticky:
                    self.showHeaders.Checked = sc.sticky['showHeadersChecked']
                else:
                    self.showHeaders.Checked = True
                self.showHeaders.Text = "Export Headers\t"
                self.showHeaders.CheckedChanged += self.showHeadersChanged

            def createColumns():
                #Column -Number
                self.numCol = forms.GridColumn()
                self.numCol.DataCell.TextAlignment = forms.TextAlignment.Right
                self.numColSortUp = True

                #Column - GUID
                self.guidCol = forms.GridColumn()
                self.guidColSortUp = True

                #Column - Type
                self.typeCol = forms.GridColumn()
                self.typeColSortUp = True

                #Column - Name
                self.nameCol = forms.GridColumn()
                self.nameCol.Sortable = True
                self.nameCol.Editable = True
                self.nameColSortUp = True

                #Column - Layer
                self.layerCol = forms.GridColumn()
                self.layerColSortUp = True

                #Column - Color
                self.colorCol = forms.GridColumn()
                self.colorColSortUp = True

                #Column - linetype
                self.linetypeCol = forms.GridColumn()
                self.linetypeColSortUp = True

                #Column - p_color
                self.p_colorCol = forms.GridColumn()
                self.p_colorColSortUp = True

                #Column - p_width
                self.p_widthCol = forms.GridColumn()
                self.p_widthCol.DataCell.TextAlignment = forms.TextAlignment.Right
                self.p_widthColSortUp = True

                #Column - Material
                self.materialCol = forms.GridColumn()
                self.materialColSortUp = True

                #Column - Length
                self.lengthCol = forms.GridColumn()
                self.lengthCol.DataCell.TextAlignment = forms.TextAlignment.Right
                self.lengthColSortUp = True

                #Column - Area
                self.areaCol = forms.GridColumn()
                self.areaCol.DataCell.TextAlignment = forms.TextAlignment.Right
                self.areaColSortUp = True

                #Column - Volume
                self.volCol = forms.GridColumn()
                self.volCol.DataCell.TextAlignment = forms.TextAlignment.Right
                self.volColSortUp = True

                #Column - Num Edges
                self.numEdgesCol = forms.GridColumn()
                self.numEdgesColSortUp = True
                #Column - Num Pts
                self.numPtsCol = forms.GridColumn()
                self.numPtsColSortUp = True
                #Column - Degree
                self.degreeCol = forms.GridColumn()
                self.degreeColSortUp = True
                #Column - Center Z
                self.centerZCol = forms.GridColumn()
                self.centerZColSortUp = True
                #Column - Is Planar
                self.isPlanarCol = forms.GridColumn()
                self.isPlanarColSortUp = True
                #Column - Is Closed
                self.isClosedCol = forms.GridColumn()
                self.isClosedColSortUp = True

            def createCheckboxSettings():
                for i in range(len(self.checkboxes)):
                    self.checkboxes[i].Text = self.colNames[i] + " \t"
                    self.checkboxes[i].CheckedChanged += self.onCheckboxChanged
                    if self.paramStates[i] in sc.sticky:
                        self.checkboxes[i].Checked = sc.sticky[self.paramStates[i]]
                    else:
                        self.checkboxes[i].Checked = True
            def createColumnSettings():
                for i in range(len(self.columns)):
                    self.columns[i].HeaderText = self.colNames[i] + " \t"
                    self.columns[i].DataCell = forms.TextBoxCell(i)
                    self.grid.Columns.Add(self.columns[i])
                for num in self.alignRightCols:
                    self.columns[num].DataCell.TextAlignment = forms.TextAlignment.Right
            
            try:
                createCheckboxes()
                createColumns()
                self.SetupLists()
                createCheckboxSettings()
                createColumnSettings()
            except:
                print "Column and Checkbox Setup failed"
            ########################################################################
            def createButtons():
                #Button - Add to selection
                self.addSelection = forms.Button()
                self.addSelection.Width = 30
                if self.addImage is None:
                    self.addSelection.Text = " + "
                else:
                    self.addSelection.Image = self.addImage
                self.addSelection.ToolTip = "Add objects to the table"
                self.addSelection.Click += self.AddByPicking

                #Button - Update Data
                self.updateSelection = forms.Button()
                if self.updateImage is None:
                    self.updateSelection.Text = "Update"
                else:
                    self.updateSelection.Image = self.updateImage
                self.updateSelection.ToolTip = "Update the table"
                self.updateSelection.Width = 30
                self.updateSelection.Click += self.Regen

                #Button - Remove from selection
                self.removeSelection = forms.Button()
                if self.removeImage is None:
                    self.removeSelection.Text = " - "
                else:
                    self.removeSelection.Image = self.removeImage
                self.removeSelection.Width = 30
                self.removeSelection.ToolTip = "Remove objects from the table"
                self.removeSelection.Click += self.RemoveByPicking

                #Button - Copy to Clipboard
                self.btnCopy = forms.Button()
                self.btnCopy.Text = "Copy"
                self.btnCopy.ToolTip = "Copy data to the clipboard"
                self.btnCopy.Click += self.copyToClipboard

                #Button - Settings
                self.btnSettings = forms.Button()
                self.btnSettings.Text = "Settings"
                self.btnSettings.ToolTip = "More Settings"
                self.btnSettings.Click += self.OnSettingsDialog

                #Button - Export Data
                self.btnExport = forms.Button()
                self.btnExport.Text = "Save as..."
                self.btnExport.ToolTip = "Export table to CSV, HTML, or TXT"
                self.btnExport.Click += self.exportData

                #Button - Close
                self.btnClose = forms.Button()
                self.btnClose.Text = "Close"
                self.btnClose.ToolTip = "Close"
                self.btnClose.Click += self.closeDialog
            createButtons()
            ########################################################################
            def createGroups():
                #Groupbox - Radio - Update
                self.updateModeGroup = forms.GroupBox(Text = "Update")
                self.updateModeGroup.Padding = drawing.Padding(5)
                self.updateModeGroup.Content = self.radioMode

                #Groupbox - Checkboxes - Properties
                self.parameterGroup = forms.GroupBox(Text = "Properties")
                self.parameterGroup.Padding = drawing.Padding(5)
                self.parameterLayout = forms.DynamicLayout()
                for checkbox in self.checkboxes:
                    self.parameterLayout.AddRow(checkbox)
                self.parameterLayout.AddRow(None)
                self.parameterGroup.Content = self.parameterLayout

                #Groupbox - Checkboxes - Options
                self.optionsGroup = forms.GroupBox(Text = "Options")
                self.optionsGroup.Padding = drawing.Padding(5)
                self.optionsLayout = forms.DynamicLayout()
                self.optionsLayout.AddRow(self.showUnits)
                self.optionsLayout.AddRow(self.showTotal)
                self.optionsLayout.AddRow(self.showHeaders)
                self.optionsLayout.AddRow(None)
                self.optionsGroup.Content = self.optionsLayout
            createGroups()
            ########################################################################
            #Dynamic Layout - gridLayout
            self.gridGroupBox = forms.GroupBox(Text = "Table")
            self.gridLayout = forms.DynamicLayout()
            self.gridLayout.AddRow(self.grid)
            self.gridLayout.AddRow(None)
            self.gridLayout.AddSeparateRow(self.addSelection, self.removeSelection,self.updateSelection, self.btnSettings, None,self.btnCopy, self.btnExport, self.btnClose)
            self.gridGroupBox.Content = self.gridLayout
            self.gridLayout.Spacing = drawing.Size(5, 5)

            #Dynamic Layout - paramPanel
            self.paramPanel = forms.DynamicLayout()
            self.paramPanel.AddSeparateRow(self.parameterGroup)
            self.paramPanel.AddSeparateRow(self.optionsGroup)
            self.paramPanel.AddSeparateRow(self.updateModeGroup)
            self.paramPanel.AddSeparateRow(self.countLbl)
            self.paramPanel.AddSeparateRow(None)

        except:
            print "CreateControls() failed"

    #Setup the lists
    def SetupLists(self):
        self.columns = [
        self.numCol,
        self.guidCol,
        self.typeCol,
        self.nameCol,
        self.layerCol,
        self.colorCol,
        self.linetypeCol,
        self.p_colorCol,
        self.p_widthCol,
        self.materialCol,
        self.lengthCol,
        self.areaCol,
        self.volCol,
        self.numEdgesCol,
        self.numPtsCol,
        self.degreeCol,
        self.centerZCol,
        self.isPlanarCol,
        self.isClosedCol,
        ]

        self.colNames = [
        "#",        #0
        "GUID",     #1
        "Type",     #2
        "Name",     #3
        "Layer",    #4
        "Color",    #5
        "Linetype", #6
        "PrintColor",  #7
        "PrintWidth",  #8
        "Material", #9
        "Length",   #10
        "Area",     #11
        "Volume",   #12
        "NumEdges", #13
        "NumPts",   #14
        "Degree",  #15
        "CenterZ",  #16
        "IsPlanar", #17
        "IsClosed"  #18
        ]

        self.checkboxes = [
        self.showNum,
        self.showGUID,
        self.showType,
        self.showName,
        self.showLayer,
        self.showColor,
        self.showLinetype,
        self.showP_Color,
        self.showP_Width,
        self.showMaterial,
        self.showLength,
        self.showArea,
        self.showVol,
        self.showNumEdges,
        self.showNumPts,
        self.showDegree,
        self.showCenterZ,
        self.showIsPlanar,
        self.showIsClosed
        ]

        self.paramStates = [
        "showNumChecked",       #0
        "showGUIDChecked",      #1
        "showTypeChecked",      #2
        "showNameChecked",      #3
        "showLayerChecked",     #4
        "showColorChecked",     #5
        "showLinetypeChecked",  #6
        "showP_ColorChecked",   #7
        "showP_WidthChecked",   #8
        "showMaterialChecked",  #9
        "showLengthChecked",    #10
        "showAreaChecked",      #11
        "showVolChecked",       #12
        "showNumEdgesChecked",  #14
        "showNumPtsChecked",    #15
        "showDegreeChecked",   #16
        "showCenterZChecked",   #17
        "showIsPlanarChecked",  #18
        "showIsClosedChecked"   #19
        ]

        self.num_Num = 0
        self.guid_Num = 1
        self.type_Num = 2
        self.name_Num = 3
        self.layer_Num = 4
        self.color_Num = 5
        self.linetype_Num = 6
        self.p_color_Num = 7
        self.p_width_Num = 8
        self.material_Num = 9
        self.length_Num = 10
        self.area_Num = 11
        self.volume_Num = 12
        self.numEdges_Num = 13
        self.numPts_Num = 14
        self.degrees_Num = 15
        self.centerZ_Num = 16
        self.isPlanar_Num = 17
        self.isClosed_Num = 18

        self.sortDirections = [
        self.numColSortUp,      #0
        self.guidColSortUp,     #1
        self.typeColSortUp,     #2
        self.nameColSortUp,     #3
        self.layerColSortUp,    #4
        self.colorColSortUp,    #5
        self.linetypeColSortUp, #6
        self.p_colorColSortUp,  #7
        self.p_widthColSortUp,  #8
        self.materialColSortUp, #9
        self.lengthColSortUp,   #10
        self.areaColSortUp,     #11
        self.volColSortUp,      #12
        self.numEdgesColSortUp, #14
        self.numPtsColSortUp,   #15
        self.degreeColSortUp,  #16
        self.centerZColSortUp,  #17
        self.isPlanarColSortUp, #18
        self.isClosedColSortUp  #19
        ]
        
        self.alignRightCols = [
        self.num_Num,
        self.p_width_Num,
        self.length_Num,
        self.area_Num,
        self.volume_Num,
        self.numEdges_Num,
        self.numPts_Num,
        self.degrees_Num,
        self.centerZ_Num
        ]

        self.typeDict = {
        0       : "Unknown object",
        1       : "Point",
        2       : "Point cloud",
        4       : "Curve",
        8       : "Surface",
        16      : "Polysurface",
        32      : "Mesh",
        256     : "Light",
        512     : "Annotation",
        4096    : "Block",
        8192    : "Text dot object",
        16384   : "Grip object",
        32768   : "Detail",
        65536   : "Hatch",
        131072  : "Morph control",
        134217728  : "Cage",
        268435456  : "Phantom",
        536870912  : "Clipping plane"
        }

    #CheckSticky
    def CheckSticky(self):
        try:
            self.CheckStickyForProperties()
            self.GetObjsFromSticky()
        except:
            print "CheckSticky() failed"

    def CheckStickyForProperties(self):
        for i, parameter in enumerate(self.paramStates):
            if str(parameter) in sc.sticky: bool = sc.sticky[str(parameter)]
            else: bool = True
            self.columns[i].Visible = bool #Show visible columns

    def SetObjs2Sticky(self):
        sc.sticky["self.objs"] = self.objs

    def GetObjsFromSticky(self):
        try:
            self.objs = sc.sticky["self.objs"]
        except:
            self.objs = []

    #CreateEvents
    def CreateEvents(self):
        Rhino.Commands.Command.EndCommand += self.OnModifyObject
        sc.doc.ActiveDoc.ModifyObjectAttributes += self.OnModifyObject

    def RemoveEvents(self):
        sc.doc.ActiveDoc.ModifyObjectAttributes -= self.OnModifyObject
        Rhino.Commands.Command.EndCommand -= self.OnModifyObject

    def OnSettingsDialog(self, sender, e):
        try:
            settingsDialog = SettingsDialog()
            settingsDialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow)
            if settingsDialog.applyBool:
                self.Regen()
        except:
            print "OnSettingsDialog() failed"

    #Change selection functions
    def AddByPicking(self, sender, e):
        try:
            rs.LockObjects(self.objs)
            newObjs = rs.GetObjects("Select objects to add to selection", preselect = True)
            rs.UnlockObjects(self.objs)
            if newObjs is None: return
            self.objs += newObjs
            #Regen
            self.Regen()
        except:
            print "AddByPicking() Failed"

    def RemoveByPicking(self, sender, e):
        try:
            #Get objs to remove
            rs.EnableRedraw(False)
            tempLock = rs.InvertSelectedObjects(rs.SelectObjects(self.objs))
            rs.LockObjects(tempLock)
            rs.EnableRedraw(True)
            items = rs.GetObjects("Select objects to remove from selection", preselect = True)
            rs.UnlockObjects(tempLock)
            if items is None: return
            for item in items:
                try:
                    self.RemoveByGUID(str(item))
                except:
                    pass
            #Regen
            self.Regen()
        except:
            print "RemoveByPicking() Failed"

    def RemoveSelection(self, sender, e):
        #Get objs to remove
        items = list(self.grid.SelectedItems)
        if items is None: return
        for item in items:
            try:
                self.RemoveByGUID(str(item[self.guid_Num]))
            except:
                pass
        #Regen
        self.Regen()

    def RemoveByGUID(self, guid):
        for i, obj in enumerate(self.objs):
            if str(obj) == str(guid):
                del self.objs[i]
                break

    def Regen(self, *args):
        #Remove blank GUIDs
        self.CleanGUIDs()
        #Regen Data
        self.RegenData()
        #Regen Format
        self.RegenFormat()

    def CleanGUIDs(self):
        try:
            existingGUIDs = []
            count = 0
            for i, obj in enumerate(self.objs):
                if rs.IsObject(obj):
                    existingGUIDs.append(obj)
                else:
                    count += 1
            if count > 0:
                print "{} objects from previous selection not found.".format(count)
            self.objs = existingGUIDs
            self.SetObjs2Sticky()
        except:
            print "CleanGUIDs() failed"

    def RegenData(self):
        try:
            def RegenDataForObj(num, obj):
                try:
                    isCurve = rs.IsCurve(obj)
                    isSurface = rs.IsSurface(obj)
                except:
                    isCurve = False
                    isSurface = False
                #Number
                try:
                    number = num+1
                except:
                    number = ""
                    print "number failed"

                #GUID
                try:
                    guid = str(obj)
                except:
                    guid = "Error"
                    print "guid failed"

                #Name
                try:
                    name = rs.ObjectName(obj)
                    if name is None: name = "<None>"
                except:
                    name = "<unnamed>"
                    print "name failed"

                #Layer
                try:
                    layer = rs.LayerName(rs.ObjectLayer(obj), False)
                except:
                    layer = "Unknown"
                    print "layer failed"

                #Color
                try:
                    if colorFormat == 0:
                        color = str(rs.ObjectColor(obj).ToKnownColor())
                        if color == str(0):
                            color = "Other"
                    if colorFormat == 1:
                        color = str(rs.ObjectColor(obj).R) + "-" +str(rs.ObjectColor(obj).G) + "-" +str(rs.ObjectColor(obj).B)
                    if colorFormat == 2:
                        color = str(rs.ObjectColor(obj).R) + "," +str(rs.ObjectColor(obj).G) + "," +str(rs.ObjectColor(obj).B)
                except:
                    color = "Unknown"
                    print "color failed"

                #Length
                try:
                    if isCurve:
                        length = str(rs.CurveLength(obj))
                    if isSurface:
                        length = ""
                    try:
                        length
                    except:
                        length = ""
                except:
                    length = None
                    print "length failed"

                #Area
                try:
                    if isCurve:
                        if rs.IsCurveClosed(obj):
                            area = str(rs.Area(obj))
                    if rs.IsBrep(obj):
                        area = str(rs.Area(obj))
                    try:
                        area
                    except:
                        area = ""
                except:
                    area = ""
                    print "area failed"

                #Volume
                try:
                    if rs.IsPolysurface(obj):
                        if rs.IsPolysurfaceClosed(obj):
                            volume = str(rs.SurfaceVolume(obj)[0])
                    try:
                        volume
                    except:
                        volume = ""
                except:
                    volume = None
                    print "volume failed"

                #Material
                try:
                    tempMat = rs.MaterialName(rs.ObjectMaterialIndex(obj))
                    if tempMat is None:
                        material = ""
                    else: material = str(tempMat)
                except:
                    material = ""
                    print "material failed"

                #TYPE
                try:
                    typeNum = rs.ObjectType(obj)
                    type = self.typeDict[typeNum]
                except:
                    type = "Extrusion"

                #Print Color
                try:
                    if colorFormat == 0:
                        p_color = str(rs.ObjectPrintColor(obj).ToKnownColor())
                        if p_color == str(0):
                            p_color = "Other"
                    if colorFormat == 1:
                        p_color = str(rs.ObjectPrintColor(obj).R) + "-" +str(rs.ObjectPrintColor(obj).G) + "-" +str(rs.ObjectPrintColor(obj).B)
                    if colorFormat == 2:
                        p_color = str(rs.ObjectPrintColor(obj).R) + "," +str(rs.ObjectPrintColor(obj).G) + "," +str(rs.ObjectPrintColor(obj).B)
                except:
                    p_color = ""
                    print "p_color failed"

                #Print Width
                try:
                    p_width = str(rs.ObjectPrintWidth(obj))
                except:
                    p_width = ""
                    print "p_width failed"

                #Linetype
                try:
                    linetype = str(rs.ObjectLinetype(obj))
                except:
                    linetype = ""
                    print "linetype failed"
                
                #NumEdges
                try:
                    if isCurve:
                        if rs.IsPolyCurve(obj):
                            numEdges = str(rs.PolyCurveCount(obj))
                        elif rs.IsPolyline(obj):
                            numEdges = str(len(rs.PolylineVertices(obj))-1)
                        else:
                            numEdges = "1"
                    if rs.IsBrep(obj):
                        numEdges = str(rs.coercebrep(obj).Edges.Count)
                    try:
                        numEdges
                    except:
                        numEdges = ""
                except:
                    numEdges = "Failed"
                    print "numEdges failed"
                
                #Num Pts
                try:
                    if isCurve:
                        if rs.IsCurveClosed(obj):
                            numPts = str(len(rs.CurveEditPoints(obj))-1)
                        else: numPts = str(len(rs.CurveEditPoints(obj)))
                    elif isSurface:
                        numPts = str(len(rs.SurfaceEditPoints(obj)))
                    elif rs.IsPoint(obj):
                        numPts = "1"
                    elif rs.IsMesh(obj):
                        numPts = rs.MeshVertexCount(obj)
                    else:
                        numPts = ""
                    try:
                        numPts
                    except:
                        numPts = ""
                except:
                    numPts = ""
                    print "numPts failed"
                
                try:
                    if isCurve:
                        degree = str(rs.CurveDegree(obj))
                    if isSurface:
                        degree = str(rs.SurfaceDegree(obj, 2))
                        degree = degree.replace(", ", "-")
                    try:
                        degree
                    except:
                        degree = ""
                except:
                    degree = ""
                    print "degree failed"
                
                try:
                    pts = rs.BoundingBox(obj)
                    pt0 = pts[0][2]
                    pt6 = pts[6][2]
                    centerZ = str((pt0 + pt6) /2)
                except:
                    centerZ = "Failed"
                    print "Center Z failed"
                    
                
                try:
                    if rs.IsCurve(obj):
                        isPlanar = str(rs.IsCurvePlanar(obj))
                    elif rs.IsSurface(obj):
                        isPlanar = str(rs.IsSurfacePlanar(obj))
                    else:
                        isPlanar = ""
                    try:
                        isPlanar
                    except:
                        isPlanar = ""
                except:
                    isPlanar = ""
                    print "isPlanar failed"

                try:
                    if isCurve:
                        isClosed = str(rs.IsCurveClosed(obj))
                    else:
                        isClosed = ""
                    if rs.IsBrep(obj):
                        if rs.IsPolysurface(obj):
                            isClosed = str(rs.IsPolysurfaceClosed(obj))
                        elif isSurface:
                            isClosed = str(rs.IsPolysurfaceClosed(obj))
                        else:
                            isClosed = ""
                    try:
                        isClosed
                    except:
                        isClosed = ""
                except:
                    isClosed = ""
                    print "isClosed failed"

                thisData = [
                number,     #0
                guid,       #1
                type,       #2
                name,       #3
                layer,      #4
                color,      #5
                linetype,   #6
                p_color,    #7
                p_width,    #8
                material,   #9
                length,     #10
                area,       #11
                volume,     #12
                numEdges,
                numPts,
                degree,
                centerZ,
                isPlanar,
                isClosed
                ]
                return thisData

            self.data = []
            for i, obj in enumerate(self.objs):
                try:
                    self.data.append(RegenDataForObj(i, obj))
                except:
                    print "RegenDataForObj failed"
            self.grid.DataStore = self.data
        except:
            print "RegenData() Failed"

    def RegenFormat(self):
        try:
            #Re-sort the data
            self.sortData()
            #Renumber
            self.renumberData()
            #Totals
            self.showTotalsFunc()
            #Thousands Separator
            self.formatNumbers()
            #Units
            self.showUnitsFunc()
            #Update Count Label
            self.recountLabel()
        except:
            print "RegenFormat() Failed"

    #Modify object functions
    def OnSelectFromGrid(self, sender, e):
        try:
            selectItems = []
            items = list(self.grid.SelectedItems)
            for item in items:
                selectItems.append(str(item[self.guid_Num]))
            rs.SelectObjects(selectItems)
        except:
            print "OnSelectFromGrid() failed"

    def OnNameCellChanged(self, sender, e):
        try:
            columnEdited = str(e.GridColumn.HeaderText).split(" ")[0]
            guid = str(rs.coerceguid(e.Item[self.guid_Num]))
            if columnEdited == "Name":
                newName = str(e.Item[self.name_Num])
                rs.ObjectName(guid, newName)
        except:
            print "OnCellChanged() failed"

    def OnNameChanged(self, sender, e):
        try:
            items = list(self.grid.SelectedItems)
            if items is None: return
            newName = rs.StringBox("New Object Name", title = "Rename Objects")
            if newName is None: return
            rs.EnableRedraw(False)
            for item in items:
                guid = item[self.guid_Num]
                rs.ObjectName(guid, newName)
            rs.EnableRedraw(True)
            if self.radioMode.SelectedIndex == 1:
                self.Regen()
        except:
            print "OnLayerChanged() Failed"

    def OnLayerChanged(self, sender, e):
        try:
            items = list(self.grid.SelectedItems)
            if items is None: return
            newLayer = rs.GetLayer()
            if newLayer is None: return
            rs.EnableRedraw(False)
            for item in items:
                guid = item[self.guid_Num]
                rs.ObjectLayer(guid, newLayer)
            rs.EnableRedraw(True)
            if self.radioMode.SelectedIndex == 1:
                self.Regen()
        except:
            print "OnLayerChanged() Failed"

    def OnColorChanged(self, sender, e):
        try:
            items = list(self.grid.SelectedItems)
            if items is None: return
            newColor = rs.GetColor()
            if newColor is None: return
            rs.EnableRedraw(False)
            for item in items:
                guid = item[self.guid_Num]
                rs.ObjectColor(guid, newColor)
            rs.EnableRedraw(True)
            if self.radioMode.SelectedIndex == 1:
                self.Regen()
        except:
            print "OnLayerChanged() Failed"

    def OnModifyObject(self, *args):
        try:
            if int(self.radioMode.SelectedIndex) == 0:
                #If automatic mode selected
                self.Regen()
        except:
            print "OnModifyObject() failed"

    def OnRadioChanged(self, sender, e):
        if self.radioMode.SelectedIndex:
            sc.sticky['radioMode'] = self.radioMode.SelectedIndex
        else:
            sc.sticky['radioMode'] = self.radioMode.SelectedIndex
            self.Regen()

    def OnDeleteRhinoObject(self, sender, e):
        try:
            self.RemoveByGUID(e.ObjectId)
        except:
            print "OnDeleteRhinoObject Failed"

    def OnSizeChanged(self, *args):
        self.grid.Height = self.Height-125
        sc.sticky['dialogSize'] = self.Size

    #Get Active
    def activeHeadingsList(self):
        currentHeadings = []
        for i, each in enumerate(self.colNames):
            if self.checkboxes[i].Checked:
                currentHeadings.append(str(each))
        return currentHeadings

    def activeDataList(self):
        currentDataIndex = []

        for i, each in enumerate(self.checkboxes):
            if each.Checked:
                currentDataIndex.append(i)

        newDataStore = []
        for row in self.grid.DataStore:
            rowAttribute = []
            for index in currentDataIndex:
                rowAttribute.append(row[index])
            newDataStore.append(rowAttribute)
        return newDataStore

    #SORT
    def sortColumn(self, sender, e):
        try:
            self.hideTotalsFunc()
            self.hideUnitsFunc()
            self.hideThousandsComma()

            colName = e.Column.HeaderText.split(" ")[0]
            for i, each in enumerate(self.colNames):
                if each == colName:
                    self.sortingBy = i
                    break
            self.sortDirections[self.sortingBy] = not self.sortDirections[self.sortingBy]

            #Headers
            self.RemoveHeaderArrows()
            self.AddHeaderArrow(self.sortingBy)
            #Regen Format
            self.RegenFormat()
        except:
            print "sortColumn() Failed"

    def sortData(self):
        try:
            try:
                self.sortingBy
            except:
                self.sortingBy = 0
            state = self.sortDirections[self.sortingBy]
            keys = []
            for row in self.grid.DataStore:
                keyRaw = row[self.sortingBy]
                try:
                    if keyRaw is not None:
                        keyFormat = float(keyRaw)
                    else:
                        keyFormat = -99999999
                except:
                    keyFormat = keyRaw.lower()
                keys.append(keyFormat)
            zipLists = zip(keys, self.grid.DataStore)
            zipLists.sort(reverse = state)
            self.grid.DataStore = [x for y, x in zipLists]
        except:
            print "sortData() Failed"

    def renumberData(self):
        try:
            data = self.grid.DataStore
            for i, row in enumerate(data):
                row[0] = str(i+1)
            self.grid.DataStore = data
        except:
            print "renumberData() Failed"

    def AddHeaderArrow(self, colNum):
        direction = self.sortDirections[colNum]
        if direction:
            self.columns[colNum].HeaderText += " ^\t"
        else:
            self.columns[colNum].HeaderText += " v\t"

    def RemoveHeaderArrows(self):
        for i in range(len(self.columns)):
            self.columns[i].HeaderText = self.columns[i].HeaderText.split(" ")[0]

    #CLOSE
    def closeDialog(self, sender, e):
        try:
            self.RemoveEvents()
        except:
            print "RemoveEvents() failed"
        try:
            sc.sticky['dialogPos'] = self.Location
            self.Close()
        except:
            print "closeDialog() failed"

    #Checkbox Events
    def onCheckboxChanged(self, sender, e):
        try:
            checkboxName = sender.Text.split(" ")[0]
            for i, item in enumerate(self.colNames):
                if checkboxName == item:
                    number = i
                    break

            sc.sticky[self.paramStates[number]] = self.checkboxes[number].Checked
            self.columns[number].Visible = self.checkboxes[number].Checked
        except:
            print "onCheckboxChanged() failed"

    def showUnitsChanged(self, sender, e):
        sc.sticky['showUnitsChecked'] = self.showUnits.Checked
        if self.showUnits.Checked:
            self.showUnitsFunc()
        else:
            self.hideUnitsFunc()

    def showTotalChanged(self, sender, e):
        sc.sticky['showTotalChecked'] = self.showTotal.Checked
        if self.showTotal.Checked:
            self.hideThousandsComma()
            self.showTotalsFunc()
            self.formatNumbers()
            self.showUnitsFunc()
        else:
            self.hideTotalsFunc()

    def showHeadersChanged(self, sender, e):
        sc.sticky['showHeadersChecked'] = self.showHeaders.Checked

    #Functions
    def formatNumbers(self):
        try:
            numColumns = [self.length_Num, self.area_Num, self.volume_Num, self.centerZ_Num]
            data = self.grid.DataStore
            for list in data:
                for numColumn in numColumns:
                    if list[numColumn] is not None:
                        if len(list[numColumn])>0:
                            if bool(commaSep):
                                list[numColumn] = "{0:,.{prec}f}".format(float(list[numColumn]), prec = int(decPlaces))
                            else:
                                list[numColumn] = "{0:.{prec}f}".format(float(list[numColumn]), prec = int(decPlaces))
            self.grid.DataStore = data
        except:
            print "Showing Thousands Separator Failed"

    def hideThousandsComma(self):
        try:
            numColumns = [self.length_Num, self.area_Num, self.volume_Num]
            data = self.grid.DataStore
            for list in data:
                for numColumn in numColumns:
                    if list[numColumn] is not None:
                        list[numColumn] = list[numColumn].replace(",", "")
        except:
            print "hideThousandsComma() Failed"

    def showUnitsFunc(self):
        if self.showUnits.Checked:
            data = self.grid.DataStore
            units = rs.UnitSystemName(False, True, True)
            try:
                for list in data:
                    #Length
                    if len(list[self.length_Num])>0:
                        list[self.length_Num] += " " + str(units)
                    #Area
                    if len(list[self.area_Num])>0:
                        list[self.area_Num] += " " + str(units) + "2"
                    #Volume
                    if len(list[self.volume_Num])>0:
                        list[self.volume_Num] += " " + str(units) + "3"
                    #Center Z
                    if len(list[self.centerZ_Num])>0:
                        list[self.centerZ_Num] += " " + str(units)
            except:
                print "showUnitsFunc() Failed"
            self.grid.DataStore = data

    def hideUnitsFunc(self):
        data = self.grid.DataStore
        try:
            for list in data:
                #Length
                if list[self.length_Num] is not None:
                    list[self.length_Num] = list[self.length_Num].split(" ")[0]
                #Area
                if list[self.area_Num] is not None:
                    list[self.area_Num] = list[self.area_Num].split(" ")[0]
                #Volume
                if list[self.volume_Num] is not None:
                    list[self.volume_Num] = list[self.volume_Num].split(" ")[0]
                #Center Z
                if list[self.centerZ_Num] is not None:
                    list[self.centerZ_Num] = list[self.centerZ_Num].split(" ")[0]
        except:
            print "hideUnitsFunc() Failed"
        self.grid.DataStore = data

    def showTotalsFunc(self):
        if self.showTotal.Checked:
            try:
                data = self.grid.DataStore
                self.hideUnitsFunc()
                totalArea = 0
                totalVolume = 0
                totalLength = 0
                for row in data:
                    if len(row[self.length_Num])>0:
                        totalLength += float(row[self.length_Num])
                    if len(row[self.area_Num])>0:
                        totalArea += float(row[self.area_Num])
                    if len(row[self.volume_Num])>0:
                        totalVolume += float(row[self.volume_Num])
                secondLastRow = []
                lastRow = []
                for i in range(len(self.grid.DataStore[0])):
                    secondLastRow.append("")
                    lastRow.append("")
                lastRow[self.num_Num] = "TOTAL"
                lastRow[self.length_Num] = str(totalLength)
                lastRow[self.area_Num] = str(totalArea)
                lastRow[self.volume_Num] = str(totalVolume)
                data.append(secondLastRow)
                data.append(lastRow)
                self.grid.DataStore = data
            except:
                print "showTotal() failed"

    def hideTotalsFunc(self):
        try:
            tempData = self.grid.DataStore
            if tempData[-1][0] == "TOTAL":
                del tempData[-1]
                del tempData[-1]
                self.grid.DataStore = tempData
            else:
                print "Already hidden totals"
                return
        except:
            print "hideTotalsFunc() failed"

    def recountLabel(self):
        self.countLbl.Text = "Selected: {} objects".format(len(self.objs))

    #I/O Functions
    def copyToClipboard(self, sender, e):
        try:
            string = self.DataStoreToHTML()
            rs.ClipboardText(string)
            print "Copied table to clipboard"
        except:
            print "copyToClipboard() Failed"

    def copySelectionToClipboard(self, sender, e):
        try:
            items = list(self.grid.SelectedItems)
            string = self.DataStoreToHTML(items)
            rs.ClipboardText(string)
            print "Copied Selection to clipboard"
        except:
            print "copySelectionToClipboard() Failed"

    def exportData(self, sender, e):
        try:
            fileName = rs.SaveFileName("Save table", filter = "CSV Files (*.csv)|*.csv|HTML Files (*.html)|*.html|TXT Files (*.txt)|*.txt||")
            extension = fileName.split(".")[-1]
            if fileName is None:
                return
            try:
                f = open(fileName,'w')
            except IOError:
                print "Cannot save file. File already open."
                return
            try:
                if extension == "html":
                    string = self.DataStoreToHTML()
                if extension == "csv":
                    global colorFormat
                    global commaSep
                    
                    prevCommaSep = commaSep
                    prevColorFormat = colorFormat
                    
                    if colorFormat == 2:
                        colorFormat = 1
                    
                    if commaSep:
                        commaSep = False
                    
                    self.Regen()
                    
                    string = self.DataStoreToCSV()
                    
                    colorFormat = prevColorFormat
                    commaSep = prevCommaSep
                    
                    self.Regen()
                if extension == "txt":
                    string = self.DataStoreToTXT()
                f.write(string)
                f.close()
                print "Exported to {} at {}".format(extension, fileName)
            except:
                print "exportData() Failed"
        except:
            pass

    def DataStoreToTXT(self):
        string = ""
        seperator = "\t"
        if self.showHeaders.Checked:
            allHeadings = self.activeHeadingsList()
            for heading in allHeadings:
                itemLen = len(str(heading))
                if itemLen >= 8:
                    numTabs = 1
                else:
                    numTabs = 2
                string += str(heading) + (seperator * numTabs)
            string += "\n"

        allData = self.activeDataList()

        for row in allData:
            for item in row:
                if str(item) == "None":
                    string += (seperator * numTabs)
                else:
                    itemLen = len(str(item))
                    if itemLen >= 8:
                        numTabs = 1
                    else:
                        numTabs = 2
                    string += str(item) + (seperator * numTabs)
            string += "\n"
        return string

    def DataStoreToCSV(self):
        string = ""
        if self.showHeaders.Checked:
            allHeadings = self.activeHeadingsList()
            print "Headings received"
            for heading in allHeadings:
                string += str(heading) + ","
            string += "\n"

        allData = self.activeDataList()

        for row in allData:
            for item in row:
                if str(item) == "None":
                    string += ","
                else:
                    string += str(item) + ","
            string += "\n"
        return string

    def DataStoreToHTML(self, *args):
        string = "<html><head><style>"
        string += "body {color: dimgray;}"
        string += "table, th, td{border-collapse: collapse; border: 1px solid black;padding: 10px;}"
        string += "</style></head><body><table>"

        if self.showHeaders.Checked:
            allHeadings = self.activeHeadingsList()
            string += "<tr>"
            for heading in allHeadings:
                string += "<th>" + str(heading) + "</th>"
            string += "</tr>"

        allData = self.activeDataList()

        #If *args specified, format them
        if len(args) > 0:
            tempItems = []
            for bracket in args:
                for item in bracket:
                    if item is not None:
                        tempItems.append(item)
            finalItems = []
            for item in tempItems:
                newItem = []
                for i, attr in enumerate(item):
                    if self.checkboxes[i].Checked:
                        newItem.append(attr)
                finalItems.append(newItem)
            allData = finalItems

        for row in allData:
            string += "<tr>"
            for item in row:
                if item is None:
                    item = ""
                string += "<td>" + str(item) + "</td>"
            string += "</tr>"
        string += "</table>"
        return string

def main():
    if 'decPlaces' in sc.sticky:
        global decPlaces
        decPlaces = sc.sticky['decPlaces']
    else:
        global decPlaces
        decPlaces = 2
        
    if 'commaSep' in sc.sticky:
        global commaSep
        commaSep = sc.sticky['commaSep']
    else:
        global commaSep
        commaSep = False
        
    if 'colorFormat' in sc.sticky:
        global colorFormat
        colorFormat = sc.sticky['colorFormat']
    else:
        global colorFormat
        colorFormat = 1
    global version
    version = "Version 0.1.0"

    Tabl = Tabl_Form()
    Tabl.Owner = Rhino.UI.RhinoEtoApp.MainWindow
    Tabl.Show()

if __name__ == "__main__":
    main()