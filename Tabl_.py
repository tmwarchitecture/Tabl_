import Rhino as rc
import Rhino.UI
import Eto.Drawing as drawing
import Eto.Forms as forms
import rhinoscriptsyntax as rs
import scriptcontext as sc
import os.path as op

class Parameter():
    def __init__(self, name, num, alignment = forms.TextAlignment.Left):
        self.name = name
        
        #Columns
        self.colNum = num
        self.col = forms.GridColumn()
        self.col.Visible = False
        self.col.HeaderText = name + " \t"
        self.col.DataCell = forms.TextBoxCell(num)
        self.col.DataCell.TextAlignment = alignment
        self.col.Sortable = True
        
        #Checkbox
        self.checkBox = forms.CheckBox()
        self.checkBox.Text = name
        self.checkBox.CheckedChanged += self.onCheckboxChanged
        #if self.paramStates[i] in sc.sticky:
        #    parameter.checkBox.Checked = sc.sticky[self.paramStates[i]]
        #else:
        #    parameter.checkBox.Checked = True
        
        #Other
        self.sortDescending = True
    
    def onCheckboxChanged(self, sender, e):
        try:
            print "{} is now {}".format(self.name, self.checkBox.Checked)
            self.col.Visible = self.checkBox.Checked
        except:
            print "onCheckboxChanged() failed"

class Settings():
    def __init__(self):
        self.version = "Version 1.1.0"
        
        #Color Format
        if 'settings.colorFormat' in sc.sticky:
            self.colorFormat = sc.sticky['settings.colorFormat']
        else:
            self.colorFormat = 0
        
        #Decimal Places
        if 'settings.decPlaces' in sc.sticky:
            self.decPlaces = sc.sticky['settings.decPlaces']
        else:
            self.decPlaces = 2
        
        #Comma Seperator
        if 'settings.commaSep' in sc.sticky:
            self.commaSep = sc.sticky['settings.commaSep']
        else:
            self.commaSep = False
        
        self.applyBool = False

class SettingsDialog(forms.Dialog):
    def __init__(self, settings):
        #Variables
        self.settings = settings
        self.applyBool = False
        self.initColorFormat = 0
        self.Title = "Settings"
        self.Padding = drawing.Padding(5)
        self.Resizable = False
        self.CreateLayout()

    def CreateLayout(self):
        self.CreateControls()
        self.colorFormatGroup = forms.GroupBox(Text = "Color Format")
        self.colorFormatLayout = forms.DynamicLayout()
        self.colorFormatLayout.AddSeparateRow(self.colorRadioBtn, None)
        self.colorFormatGroup.Content = self.colorFormatLayout

        self.numFormatGroup = forms.GroupBox(Text = "Number Format")
        self.numFormatLayout = forms.DynamicLayout()
        self.numFormatLayout.Spacing = drawing.Size(5, 5)
        self.numFormatLayout.AddSeparateRow(self.label1, None, self.numericDecPlaces)
        self.numFormatLayout.AddSeparateRow(self.label2, None, self.seperatorDropDown)
        #self.numFormatLayout.AddSeparateRow(self.showCommas, None)
        self.numFormatGroup.Content = self.numFormatLayout
        
        layout = forms.DynamicLayout()
        layout.AddSeparateRow(self.numFormatGroup, None)
        layout.AddSeparateRow(self.colorFormatGroup)
        layout.AddSeparateRow(self.optionsGroup)
        layout.AddSeparateRow(self.updateModeGroup)
        layout.AddSeparateRow(None, self.btnCancel, self.btnApply)
        layout.AddRow(None)
        layout.Spacing = drawing.Size(5, 5)

        self.Content = layout
        
        print "Creating Layout"

    def CreateControls(self):
        print "Creating Controls"
        self.label1 = forms.Label()
        self.label1.Text = "Decimal Places"
        
        self.label2 = forms.Label()
        self.label2.Text = "Thousands Separator\t"
        
        self.btnApply = forms.Button()
        self.btnApply.Text = "OK"
        self.btnApply.Click += self.OnApplySettings
        
        self.btnCancel = forms.Button()
        self.btnCancel.Text = "Cancel"
        self.btnCancel.Click += self.OnCancelSettings
        ########################################################################
        
        #Seperator
        self.seperatorDropDown = forms.DropDown()
        self.seperatorDropDown.DataStore = ['None','Comma\t","', 'Dot\t"."', 'Space\t" "']
        self.seperatorDropDown.SelectedIndex = self.settings.commaSep
        
        #Dec Places
        self.numericDecPlaces = forms.NumericUpDown()
        self.numericDecPlaces.DecimalPlaces  = 0
        self.numericDecPlaces.MaxValue = 8
        self.numericDecPlaces.MinValue = 0
        self.numericDecPlaces.Value = self.settings.decPlaces
        
        #Color Format
        self.colorRadioBtn = forms.RadioButtonList()
        self.colorRadioBtn.DataStore = ["Name", "R-G-B", "R,G,B"]
        self.colorRadioBtn.Orientation = forms.Orientation.Vertical
        self.colorRadioBtn.SelectedIndex = self.settings.colorFormat
        
        ########################################################################
        #Checkbox - Show Units
        self.showUnits = forms.CheckBox()
        if 'showUnitsChecked' in sc.sticky:
            self.showUnits.Checked = sc.sticky['showUnitsChecked']
        else:
            self.showUnits.Checked = False
        self.showUnits.Text = "Show Units\t"
        #self.showUnits.CheckedChanged += self.showUnitsChanged

        #Checkbox - Show Total
        self.showTotal = forms.CheckBox()
        if 'showTotalChecked' in sc.sticky:
            self.showTotal.Checked = sc.sticky['showTotalChecked']
        else:
            self.showTotal.Checked = False
        self.showTotal.Text = "Show Total\t"
        #self.showTotal.CheckedChanged += self.showTotalChanged

        #Checkbox - Show Headers
        self.showHeaders = forms.CheckBox()
        if 'showHeadersChecked' in sc.sticky:
            self.showHeaders.Checked = sc.sticky['showHeadersChecked']
        else:
            self.showHeaders.Checked = True
        self.showHeaders.Text = "Export Headers\t"
        #self.showHeaders.CheckedChanged += self.showHeadersChanged
        
        ########################################################################
        #Radiobutton - Auto-manual
        self.radioMode = forms.RadioButtonList()
        self.radioMode.DataStore = ["Automatic", "Manual"]
        self.radioMode.Orientation = forms.Orientation.Vertical
        try:
            self.radioMode.SelectedIndex = sc.sticky['radioMode']
        except:
            self.radioMode.SelectedIndex = 1
        #self.radioMode.SelectedIndexChanged += self.OnRadioChanged
        
        ########################################################################
        #Groupbox - Checkboxes - Options
        self.optionsGroup = forms.GroupBox(Text = "Options")
        self.optionsGroup.Padding = drawing.Padding(5)
        self.optionsLayout = forms.DynamicLayout()
        self.optionsLayout.AddRow(self.showUnits)
        self.optionsLayout.AddRow(self.showTotal)
        self.optionsLayout.AddRow(self.showHeaders)
        #self.optionsLayout.AddRow(self.showPreview)
        self.optionsLayout.AddRow(None)
        self.optionsGroup.Content = self.optionsLayout
        
        #Groupbox - Radio - Update
        self.updateModeGroup = forms.GroupBox(Text = "Update")
        self.updateModeGroup.Padding = drawing.Padding(5)
        self.updateModeGroup.Content = self.radioMode

    def OnApplySettings(self, sender, e):
        try:
            print "Applying Settings"
            self.settings.decPlaces = self.numericDecPlaces.Value
            sc.sticky['settings.decPlaces'] = self.settings.decPlaces
            
            self.settings.commaSep = self.seperatorDropDown.SelectedIndex
            sc.sticky['settings.commaSep'] = self.settings.commaSep
            
            self.settings.colorFormat = self.colorRadioBtn.SelectedIndex
            sc.sticky['settings.colorFormat'] = self.settings.colorFormat
            
            self.applyBool = True
            self.Close()
        except:
            print "Failed to apply settings"

    def OnCancelSettings(self, sender, e):
        try:
            print "Cancelled"
            self.Close()
        except:
            print "Failed to cancel settings"


class Tabl_Form(forms.Form):
    def __init__(self):
        ########################################################################
        self.SetupParameters()
        #Setup Form
        self.Initialize()
        #Create Controls
        self.CreateLayout()
        #Check Sticky
        #self.CheckSticky()
        #Setup Grid
        self.Regen()
        #Events
        self.CreateEvents()
    
    #################################TEST
    def SetupParameters(self):
        self.objs = []
        self.guids = []
        
        self.parameters = []
        self.parameters.append(Parameter("#", 0, forms.TextAlignment.Right))
        self.parameters.append(Parameter("GUID", 1))
        self.parameters.append(Parameter("Type", 2))
        self.parameters.append(Parameter("Name", 3))
        self.parameters.append(Parameter("Layer", 4))
        self.parameters.append(Parameter("Color", 5))
        self.parameters.append(Parameter("Linetype", 6))
        self.parameters.append(Parameter("PrintColor", 7))
        self.parameters.append(Parameter("PrintWidth", 8))
        self.parameters.append(Parameter("Material", 9))
        self.parameters.append(Parameter("Length", 10))
        self.parameters.append(Parameter("Area", 11, forms.TextAlignment.Right))
        self.parameters.append(Parameter("Volume", 12, forms.TextAlignment.Right))
        self.parameters.append(Parameter("NumEdges", 13, forms.TextAlignment.Right))
        self.parameters.append(Parameter("NumPts", 14, forms.TextAlignment.Right))
        self.parameters.append(Parameter("Degree", 15, forms.TextAlignment.Right))
        self.parameters.append(Parameter("CenterZ", 16, forms.TextAlignment.Right))
        self.parameters.append(Parameter("IsPlanar", 17))
        self.parameters.append(Parameter("IsClosed", 18))
    
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
        
        #Settings
        self.settings = Settings()

        ########################################################################
        #Form Icon
        def loadImages():
            path = rc.PlugIns.PlugIn.PathFromName("Tabl_")
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
                self.grid.AllowColumnReordering = True
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
            try:
                #Create Columns
                for parameter in self.parameters:
                    self.grid.Columns.Add(parameter.col)
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
                #Groupbox - Checkboxes - Properties
                self.parameterGroup = forms.GroupBox(Text = "Properties")
                self.parameterGroup.Padding = drawing.Padding(5)
                self.parameterLayout = forms.DynamicLayout()
                for parameter in self.parameters:
                    #parameter.checkBox = forms.CheckBox()
                    self.parameterLayout.AddRow(parameter.checkBox)
                self.parameterLayout.AddRow(None)
                self.parameterGroup.Content = self.parameterLayout

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
        sc.sticky["self.guids"] = self.guids
    
    def GetObjsFromSticky(self):
        try:
            self.guids = sc.sticky["self.guids"]
        except:
            self.guids = []
    
    #CreateEvents
    def CreateEvents(self):
        rc.Commands.Command.EndCommand += self.OnModifyObject
        sc.doc.ActiveDoc.ModifyObjectAttributes += self.OnModifyObject

    def OnSettingsDialog(self, sender, e):
        try:
            settingsDialog = SettingsDialog(self.settings)
            settingsDialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow)
            #if settingsDialog.applyBool:
            #    self.colorFormat = settingsDialog.colorRadioBtn.SelectedIndex
            #    self.Regen()
        except:
            print "OnSettingsDialog() failed"
            #print self.settingsDialog

    #Change selection functions
    def AddByPicking(self, sender, e):
        try:
            rs.LockObjects(self.guids)
            newIDs = rs.GetObjects("Select objects to add to selection", preselect = True)
            rs.UnlockObjects(self.guids)
            if newIDs is None: return
            self.guids += newIDs
            for id in newIDs:
                self.objs.append(rs.coercerhinoobject(id))
            #Regen
            self.Regen()
        except:
            print "AddByPicking() Failed"

    def RemoveByPicking(self, sender, e):
        try:
            #Get objs to remove
            rs.EnableRedraw(False)
            tempLock = rs.InvertSelectedObjects(rs.SelectObjects(self.guids))
            rs.LockObjects(tempLock)
            rs.EnableRedraw(True)
            items = rs.GetObjects("Select objects to remove from selection", preselect = True)
            rs.UnlockObjects(tempLock)
            if items is None: return
            for item in items:
                if item in self.guids:
                    self.guids.remove(item)
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
            for i, obj in enumerate(self.guids):
                if rs.IsObject(obj):
                    existingGUIDs.append(obj)
                else:
                    count += 1
            if count > 0:
                print "{} objects from previous selection not found.".format(count)
            self.guids = existingGUIDs
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
            for i, obj in enumerate(self.guids):
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

    def OnSizeChanged(self, *args):
        self.grid.Height = self.Height-125
        sc.sticky['dialogSize'] = self.Size

    #Get Active
    def activeHeadingsList(self):
        currentHeadings = []
        for i, each in enumerate(self.colNames):
            #if self.checkboxes[i].Checked:
            currentHeadings.append(str(each))
        return currentHeadings

    def activeDataList(self):
        currentDataIndex = []
        
        for i, each in enumerate(self.parameters):
            if each.checkBox.Checked:
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
            print "1"
            state = self.sortDirections[self.sortingBy]
            print "2"
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
            sc.doc.ActiveDoc.ModifyObjectAttributes -= self.OnModifyObject
            rc.Commands.Command.EndCommand -= self.OnModifyObject
        except:
            print "RemoveEvents() failed"
        try:
            sc.sticky['dialogPos'] = self.Location
            self.Close()
        except:
            print "closeDialog() failed"

    #Checkbox Events
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
        self.countLbl.Text = "Selected: {} objects".format(len(self.guids))

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
                    tempCommaSep = self.settings.commaSep
                    tempColorFormat = self.settings.colorFormat
                    
                    if tempColorFormat == 2:
                        tempColorFormat = 1
                    
                    #This needs to be fixed. Uses different comma sep index, not bool
                    #if tempCommaSep:
                    #    tempCommaSep = False
                    
                    self.Regen()
                    
                    string = self.DataStoreToCSV()
                    
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
    Tabl = Tabl_Form()
    Tabl.Owner = rc.UI.RhinoEtoApp.MainWindow
    Tabl.Show()

if __name__ == "__main__":
    main()