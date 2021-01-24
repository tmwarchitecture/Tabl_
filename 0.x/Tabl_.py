import Rhino as rc
import Rhino.UI
import Rhino.Geometry as rg
import Eto.Drawing as drawing
import Eto.Forms as forms
import rhinoscriptsyntax as rs
import scriptcontext as sc
import os.path as op
import System.Drawing.Color as color


class Parameter():
    def __init__(self, tabl, name, num, alignment = forms.TextAlignment.Left, vis = False, edit = False, totalable = False, units = None, is_float = False, superscript = ''):
        self.tabl = tabl
        self.name = name
        
        #Columns
        self.colNum = num
        self.col = forms.GridColumn()
        self.col.Visible = vis
        self.col.HeaderText = name + " \t"
        self.col.DataCell = forms.TextBoxCell(self.colNum)
        self.col.DataCell.TextAlignment = alignment
        self.col.Sortable = True
        self.col.Editable = edit
        
        #Checkbox
        self.checkBox = forms.CheckBox()
        self.checkBox.Text = name
        self.checkBox.Checked = vis
        self.checkBox.CheckedChanged += self.onCheckboxChanged
        
        #Other
        self.sortDescending = True
        self.totalable = totalable
        self.units = units
        self.superscript = superscript
        self.is_float = is_float
        
        self.formula = None
    
    def onCheckboxChanged(self, sender, e):
        try:
            self.col.Visible = self.checkBox.Checked
        except:
            print "onCheckboxChanged() failed"

class Settings():
    def __init__(self):
        self.version = "Version 0.2"
        self.dialogPosX = 100
        self.dialogPosY = 100
        self.dialogWidth = 600
        self.dialogHeight = 700
        
        self.guids = []
        self.objs = []
        self.parameters = []
        self.colorFormat = 0
        self.decPlaces = 2
        self.commaSep = 1
        self.showUnits = False
        self.showTotals = False
        self.showHeaders = True
        self.updateMode = 1
        self.unitScale = 1
        self.customUnit = ''
        
        self.sortingBy = 0
        
        self.place_Type = 0
        self.place_BufferSize = 2
        self.place_ColWidth = 100
        self.place_TableWidth = 30

class SettingsDialog(forms.Dialog):
    def __init__(self, settings):
        #Variables
        self.settings = settings
        self.Title = "Settings"
        self.Padding = drawing.Padding(5)
        self.Resizable = False
        self.CreateLayout()
        
        #Icon
        path = rc.PlugIns.PlugIn.PathFromName("Tabl_")
        dirName = op.dirname(path)
        try:
            iconPath = op.join(dirName, r'main.ico')
            self.Icon = drawing.Icon(iconPath)
        except:
            print "Could not load mainIcon.ico"

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
        self.numFormatGroup.Content = self.numFormatLayout
        
        ########################################################################
        #Groupbox - Custom Scales
        self.customScaleGroup = forms.GroupBox(Text = "Custom Units")
        self.customScaleLayout = forms.DynamicLayout()
        self.customScaleLayout.Spacing = drawing.Size(5, 5)
        self.customScaleLayout.AddSeparateRow(self.labelScaleUnits, None, self.numericScaleUnits)
        self.customScaleLayout.AddSeparateRow(self.labelCustomUnit, None, self.txtCustomUnits)
        self.customScaleGroup.Content = self.customScaleLayout
        
        layout = forms.DynamicLayout()
        layout.AddSeparateRow(self.numFormatGroup, None)
        layout.AddSeparateRow(self.colorFormatGroup)
        layout.AddSeparateRow(self.updateModeGroup)
        layout.AddSeparateRow(self.customScaleGroup)
        layout.AddSeparateRow(self.versionLabel)
        layout.AddSeparateRow(None, self.btnCancel, self.btnApply)
        layout.AddRow(None)
        layout.Spacing = drawing.Size(5, 5)

        self.Content = layout

    def CreateControls(self):
        self.label1 = forms.Label()
        self.label1.Text = "Decimal Places"
        
        self.labelScaleUnits = forms.Label()
        self.labelScaleUnits.Text = "Scale Units"
        
        self.labelCustomUnit = forms.Label()
        self.labelCustomUnit.Text = "Custom Unit Name"
        
        self.label2 = forms.Label()
        self.label2.Text = "Thousands Separator\t"
        
        self.versionLabel = forms.Label()
        self.versionLabel.Text = "     " + self.settings.version
        
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
        self.numericDecPlaces.ToolTip = "Changes the number of decimal places to display"
        
        #Color Format
        self.colorRadioBtn = forms.RadioButtonList()
        self.colorRadioBtn.DataStore = ["Name", "R-G-B", "R,G,B"]
        self.colorRadioBtn.Orientation = forms.Orientation.Vertical
        self.colorRadioBtn.SelectedIndex = self.settings.colorFormat
        
        #Scale Units
        self.numericScaleUnits = forms.NumericUpDown()
        self.numericScaleUnits.DecimalPlaces  = 6
        self.numericScaleUnits.MaxValue = 99999
        self.numericScaleUnits.MinValue = -1
        self.numericScaleUnits.Value = self.settings.unitScale
        self.numericScaleUnits.ToolTip = "Multiplies length by this value, squares area, and cubes volume"
        
        self.txtCustomUnits = forms.TextBox()
        self.txtCustomUnits.Text = self.settings.customUnit
        self.txtCustomUnits.TextAlignment = forms.TextAlignment.Right
        #self.txtCustomUnits.Increment = 1
        
        ########################################################################
        #Radiobutton - Auto-manual
        self.updateModeRadio = forms.RadioButtonList()
        self.updateModeRadio.DataStore = ["Automatic", "Manual"]
        self.updateModeRadio.Orientation = forms.Orientation.Vertical
        self.updateModeRadio.SelectedIndex = self.settings.updateMode
        
        ########################################################################
        #Groupbox - Radio - Update
        self.updateModeGroup = forms.GroupBox(Text = "Update")
        self.updateModeGroup.Padding = drawing.Padding(5)
        self.updateModeGroup.Content = self.updateModeRadio
        
    def OnApplySettings(self, sender, e):
        try:
            self.settings.decPlaces = self.numericDecPlaces.Value
            self.settings.commaSep = self.seperatorDropDown.SelectedIndex
            self.settings.colorFormat = self.colorRadioBtn.SelectedIndex
            self.settings.updateMode = self.updateModeRadio.SelectedIndex
            self.settings.unitScale = self.numericScaleUnits.Value
            if self.txtCustomUnits.Text.isspace():
                self.settings.customUnit = ''
            else:
                self.settings.customUnit = self.txtCustomUnits.Text
            self.Result = True
            self.Close()
        except:
            print "Failed to apply settings"

    def OnCancelSettings(self, sender, e):
        try:
            self.Result = False
            self.Close()
        except:
            print "Failed to cancel settings"

class PlaceDialog(forms.Dialog):
    def __init__(self, currentData, settings):
        self.settings = settings
        self.data = currentData
        #Variables
        self.Title = "Place Settings"
        self.Padding = drawing.Padding(5)
        self.Resizable = False
        self.CreateLayout()
        
        #Icon
        path = rc.PlugIns.PlugIn.PathFromName("Tabl_")
        dirName = op.dirname(path)
        try:
            iconPath = op.join(dirName, r'main.ico')
            self.Icon = drawing.Icon(iconPath)
        except:
            print "Could not load mainIcon.ico"
    
    def CreateLayout(self):
        self.CreateControls()
        
        self.typeRadioGroup = forms.GroupBox(Text = "Column Size")
        self.typeRadioLayout = forms.DynamicLayout()
        self.typeRadioLayout.AddSeparateRow(self.PlaceTypeRadio, None)
        self.typeRadioGroup.Content = self.typeRadioLayout
        
        self.placeFormatGroup = forms.GroupBox(Text = "Spacing")
        self.placeFormatLayout = forms.DynamicLayout()
        self.placeFormatLayout.AddSeparateRow(self.bufferUpDown,self.bufferUpDownLbl, None)
        self.placeFormatGroup.Content = self.placeFormatLayout
        
        layout = forms.DynamicLayout()
        layout.AddSeparateRow(self.typeRadioGroup)
        layout.AddSeparateRow(self.placeFormatGroup)
        layout.AddSeparateRow(None, self.btnCancel, self.PlaceBtn)
        self.Content = layout
    
    def CreateControls(self):
        self.PlaceBtn = forms.Button()
        self.PlaceBtn.Text = "Place"
        self.PlaceBtn.ToolTip = "Place Tabl inside Rhino"
        self.PlaceBtn.Click += self.OnPlace

        self.btnCancel = forms.Button()
        self.btnCancel.Text = "Cancel"
        self.btnCancel.Click += self.OnCancelSettings
        
        
        self.bufferUpDownLbl = forms.Label(Text = " Buffer Size")
        self.bufferUpDown = forms.NumericUpDown()
        self.bufferUpDown.DecimalPlaces = 2
        self.bufferUpDown.Increment = .1
        self.bufferUpDown.MinValue = 0
        self.bufferUpDown.Value = self.settings.place_BufferSize
        
        self.PlaceTypeRadio = forms.RadioButtonList()
        self.PlaceTypeRadio.DataStore = [
        "Fit Columns To Data", 
        "Fit Columns To Table Width",
        "Fixed Table Width (Divide Even)", 
        "Fixed Column Width"]
        self.PlaceTypeRadio.Orientation = forms.Orientation.Vertical
        self.PlaceTypeRadio.SelectedIndex = self.settings.place_Type

    def OnCancelSettings(self, sender, e):
        try:
            self.Close()
        except:
            print "Failed to cancel settings"

    #Place Table
    def OnPlace(self, sender, e):
        try:
            self.Close()
            result = self.PlaceTable(self.PlaceTypeRadio.SelectedIndex)
            if result is not None:
                self.settings.place_Type = self.PlaceTypeRadio.SelectedIndex
                self.settings.place_BufferSize = self.bufferUpDown.Value
        except:
            print "PlaceTable failed"
    
    def PlaceTable(self, type):
        """
        Places Tabl according to type specified.
        Type 0 = Fit Columns To Data
        Type 1 = Fit Columns To Table Width
        Type 2 = Fixed Table Width
        Type 3 = Fixed Column Width
        """
        
        def placeTable_FitWidth(dataLen, bufferSize):
            minColWidths = []
            transposed = zip(*dataLen)
            for item in transposed:
                minColWidth = None
                for param in item:
                    if param > minColWidth or minColWidth is None:
                        minColWidth = param
                minColWidths.append(minColWidth)
            return minColWidths
        
        def placeTable_FitWidthToTable(tableWidth, dataLen, bufferSize):
            def mapFromTo(x,a,b,c,d):
                y=(x-a)/(b-a)*(d-c)+c
                return y
            
            minColWidths = []
            colWidths = []
            transposed = zip(*dataLen)
            for item in transposed:
                minColWidth = 0
                for param in item:
                    if param > minColWidth:
                        minColWidth = param
                minColWidths.append(minColWidth)
            count = 0
            for width in minColWidths:
                count += width
            for width in minColWidths:
                tempWidth = mapFromTo(width, 0, count, 0, tableWidth)
                colWidths.append(tempWidth)
            return colWidths
        
        def placeTable_FixedTableWidth(tableWidth, dataLen):
            fixedWidths = []
            transposed = zip(*dataLen)
            count = 0
            for each in transposed:
                count += 1
            numCols = count
            fixedWidth = tableWidth/numCols
            for item in transposed:
                fixedWidths.append(fixedWidth)
            return fixedWidths
        
        def placeTable_FixedColWidth(fixedWidth, dataLen):
            fixedWidths = []
            transposed = zip(*dataLen)
            for item in transposed:
                fixedWidths.append(fixedWidth)
            return fixedWidths
        
        def GetRectangleLocation(width, height, bufferWidth, bufferHeight, type):
            def GetPointDynamicDrawFunc( sender, args ):
                tempWidth = width
                tempHeight = height
                
                if sc.doc.Views.ModelSpaceIsActive:
                    if type == 0:
                        tempWidth *= sc.doc.ActiveDoc.DimStyles.Current.DimensionScale
                    tempHeight *= sc.doc.ActiveDoc.DimStyles.Current.DimensionScale
                
                tempWidth += bufferWidth
                tempHeight += bufferHeight
                
                xLoc = args.CurrentPoint[0]
                yLoc = args.CurrentPoint[1]
                zLoc = args.CurrentPoint[2]
                pt0 = rg.Point3d(xLoc,yLoc,zLoc)
                pt1 = rg.Point3d(xLoc+tempWidth,yLoc,zLoc)
                pt2 = rg.Point3d(xLoc+tempWidth,yLoc-tempHeight,zLoc)
                pt3 = rg.Point3d(xLoc,yLoc-tempHeight,zLoc)
                args.Display.DrawPolyline([pt0, pt1, pt2, pt3, pt0], color.Gray, 1)
            
            gp = Rhino.Input.Custom.GetPoint()
            gp.SetCommandPrompt("Choose point to place Tabl_")
            gp.DynamicDraw += GetPointDynamicDrawFunc
            gp.Get()
            
            if (gp.CommandResult() == Rhino.Commands.Result.Success):
                pt = gp.Point()
                modelSpace = False
                if sc.doc.Views.ModelSpaceIsActive:
                    modelSpace = True
                return pt, modelSpace
        
        #Variables
        textHeight = sc.doc.ActiveDoc.DimStyles.Current.TextHeight
        bufferSize = self.bufferUpDown.Value
        
        #Check if any data
        if len(self.data) == 0:
            print "No data to place"
            return None
        
        #Get text alignment
        alignments = []
        for parameter in self.settings.parameters:
            if parameter.col.Visible:
                if parameter.col.DataCell.TextAlignment == forms.TextAlignment.Left:
                    alignments.append(rg.TextJustification.Left)
                elif parameter.col.DataCell.TextAlignment == forms.TextAlignment.Center:
                    alignments.append(rg.TextJustification.Center)
                elif parameter.col.DataCell.TextAlignment == forms.TextAlignment.Right:
                    alignments.append(rg.TextJustification.Right)
        
        #Cleanup Data
        cleanData = []
        for item in self.data:
            tempList = []
            for param in item:
                if param is None:
                    tempList.append(" ")
                elif len(str(param))<1:
                    tempList.append(" ")
                else:
                    tempList.append(str(param).rstrip())
            cleanData.append(tempList)
        
        #Get textWidths
        textWidths = []
        plane = rg.Plane.WorldXY
        dimStyle = sc.doc.ActiveDoc.DimStyles.CurrentDimensionStyle
        for item in cleanData:
            itemLen = []
            for param in item:
                text = str(param)
                textObj = rg.TextEntity.Create(text, plane, dimStyle, False,0,0)
                length = textObj.TextModelWidth
                itemLen.append(length)
            textWidths.append(itemLen)
        
        #Get colWidths
        if type == 0:
            colWidths = placeTable_FitWidth(textWidths, bufferSize)
        if type == 1:
            tableWidth = rs.GetReal("Table Width", number = self.settings.place_TableWidth, minimum = 1)
            if tableWidth is None: return None
            self.settings.place_TableWidth = tableWidth
            colWidths = placeTable_FitWidthToTable(tableWidth, textWidths, bufferSize)
        if type == 2:
            tableWidth = rs.GetReal("Table Width", number = self.settings.place_TableWidth, minimum = 1)
            if tableWidth is None: return None
            self.settings.place_TableWidth = tableWidth
            colWidths = placeTable_FixedTableWidth(tableWidth, textWidths)
        if type == 3:
            fixedWidth = rs.GetReal("Column Width", number = self.settings.place_ColWidth, minimum = 1)
            if fixedWidth is None: return None
            self.settings.place_ColWidth = fixedWidth
            colWidths = placeTable_FixedColWidth(fixedWidth, textWidths)
        
        
        #Get Tabl width, height
        totalTextWidth = 0
        for col in colWidths:
            totalTextWidth += col
        totalBufferWidth = len(colWidths) * (bufferSize * 2)
        if type > 0:
            totalBufferWidth = 0
        
        totalTextHeight = 0
        for each in cleanData:
            totalTextHeight += textHeight
        totalBufferHeight = len(cleanData) * (bufferSize * 2)
        
        #Get placePt
        placePt, modelSpace = GetRectangleLocation(totalTextWidth,totalTextHeight, totalBufferWidth, totalBufferHeight, type)
        if placePt is None: return None
        
        #Check which space it's in
        if modelSpace:
            rowHeight = (textHeight * sc.doc.ActiveDoc.DimStyles.Current.DimensionScale) + (bufferSize*2)
            if  type == 0:
                for i, colWidth in enumerate(colWidths):
                    colWidth *= sc.doc.ActiveDoc.DimStyles.Current.DimensionScale
                    colWidths[i] = colWidth + (bufferSize*2)
            else:
                for i, colWidth in enumerate(colWidths):
                    colWidths[i] = colWidth
        else:
            #In paperspace
            rowHeight = textHeight + (bufferSize*2)
            if  type == 0:
                for i, colWidth in enumerate(colWidths):
                    colWidths[i] = colWidth + (bufferSize*2)
            else:
                for i, colWidth in enumerate(colWidths):
                    colWidths[i] = colWidth
        
        #Add Text and Rectangles
        xLoc = placePt[0]
        yLoc = placePt[1]
        zLoc = placePt[2]
        allText = []
        allRect = []
        plane = rg.Plane.WorldXY
        sn = sc.doc.BeginUndoRecord("Place Tabl_")
        rs.EnableRedraw(False)
        for i, item in enumerate(cleanData):
            for j, param in enumerate(item):
                #Add rectangle
                framePt = rg.Point3d(xLoc, yLoc, zLoc)
                plane.Origin = framePt
                width = colWidths[j]
                height = rowHeight
                allRect.append(rs.AddRectangle(plane, width, -height))
                
                #Skip text with 0 length
                if len(str(param)) == 0:
                    continue
                
                #Justification
                if i == 0 and self.settings.showHeaders == True:
                    #HEADERS
                    plane.Origin = rg.Point3d(xLoc+(width/2), yLoc-bufferSize, zLoc)
                    textObject = rg.TextEntity.Create(str(param), plane, dimStyle, False,0,0)
                    textObject.Justification = rg.TextJustification.Center
                    textObject.SetBold(True)
                elif i == len(cleanData)-1 and self.settings.showTotals == True:
                    #TOTALS
                    plane.Origin = rg.Point3d(xLoc+width-bufferSize, yLoc-bufferSize, zLoc)
                    textObject = rg.TextEntity.Create(str(param), plane, dimStyle, False,0,0)
                    textObject.Justification = rg.TextJustification.Right
                else:
                    #DATA
                    if alignments[j] == rg.TextJustification.Left:
                        #LEFT
                        plane.Origin = rg.Point3d(xLoc+bufferSize, yLoc-bufferSize, zLoc)
                        textObject = rg.TextEntity.Create(str(param), plane, dimStyle, False,0,0)
                        textObject.Justification = rg.TextJustification.Left
                    elif alignments[j] == rg.TextJustification.Right:
                        #RIGHT
                        plane.Origin = rg.Point3d(xLoc+width-bufferSize, yLoc-bufferSize, zLoc)
                        textObject = rg.TextEntity.Create(str(param), plane, dimStyle, False,0,0)
                        textObject.Justification = rg.TextJustification.Right
                    else:
                        #CENTER
                        plane.Origin = rg.Point3d(xLoc+(width/2), yLoc-bufferSize, zLoc)
                        textObject = rg.TextEntity.Create(str(param), plane, dimStyle, False,0,0)
                        textObject.Justification = rg.TextJustification.Center
                
                txt = sc.doc.Objects.AddText(textObject)
                allText.append(txt)
                
                xLoc += colWidths[j]
            yLoc -= rowHeight
            xLoc = placePt[0]
        
        rs.EnableRedraw(True)
        sc.doc.Views.Redraw()
        if (sn > 0):
            sc.doc.EndUndoRecord(sn)
        return allText, allRect

class Tabl_Form(forms.Form):
    def __init__(self):
        ########################################################################
        #Settings
        if 'tabl.settings' in sc.sticky:
            self.settings = sc.sticky['tabl.settings']
        else:
            self.settings = Settings()
            self.settings.parameters = self.SetupParameters()
        
        #Setup Form
        self.Initialize()
        
        #Create Controls
        self.CreateLayout()
        
        #Setup Grid
        self.Regen()
        
        #Setup Events
        self.CreateEvents()
    
    #################################
    def SetupParameters(self):
        unit = sc.doc.GetUnitSystemName(True, False, True, True)
        parameters = []
        parameters.append(Parameter(self, "#", len(parameters), forms.TextAlignment.Right, vis = True))
        parameters.append(Parameter(self, "GUID", len(parameters)))
        parameters.append(Parameter(self, "Type", len(parameters)))
        parameters.append(Parameter(self, "Name", len(parameters), vis = True, edit = True))
        parameters.append(Parameter(self, "Layer", len(parameters), vis = True))
        parameters.append(Parameter(self, "Color", len(parameters), vis = True))
        parameters.append(Parameter(self, "Linetype", len(parameters)))
        parameters.append(Parameter(self, "PrintColor", len(parameters)))
        parameters.append(Parameter(self, "PrintWidth", len(parameters), forms.TextAlignment.Right))
        parameters.append(Parameter(self, "Material", len(parameters)))
        parameters.append(Parameter(self, "Length", len(parameters), forms.TextAlignment.Right, totalable = True, units = unit, is_float = True))
        parameters.append(Parameter(self, "Area", len(parameters), forms.TextAlignment.Right, totalable = True, units = unit, is_float = True, superscript = u'\xb2'))
        parameters.append(Parameter(self, "Volume", len(parameters), forms.TextAlignment.Right, totalable = True, units = unit, is_float = True, superscript = u'\xb3'))
        parameters.append(Parameter(self, "NumPts", len(parameters), forms.TextAlignment.Right, totalable = True))
        parameters.append(Parameter(self, "NumEdges", len(parameters), forms.TextAlignment.Right, totalable = True))
        parameters.append(Parameter(self, "NumFaces", len(parameters), forms.TextAlignment.Right, totalable = True))
        parameters.append(Parameter(self, "Degree", len(parameters), forms.TextAlignment.Right))
        parameters.append(Parameter(self, "CenterX", len(parameters), forms.TextAlignment.Right, is_float = True))
        parameters.append(Parameter(self, "CenterY", len(parameters), forms.TextAlignment.Right, is_float = True))
        parameters.append(Parameter(self, "CenterZ", len(parameters), forms.TextAlignment.Right, is_float = True))
        parameters.append(Parameter(self, "IsPlanar", len(parameters)))
        parameters.append(Parameter(self, "IsClosed", len(parameters)))
        parameters.append(Parameter(self, "Comments", len(parameters), edit = True))
        return parameters
    
    #Initialize
    def Initialize(self):
        ########################################################################
        #Form Setup
        self.Title = "Tabl_"
        self.Padding = 10
        self.Resizable = True
        self.Location = drawing.Point(self.settings.dialogPosX, self.settings.dialogPosY)
        self.Size = drawing.Size(self.settings.dialogWidth,self.settings.dialogHeight)
        self.MinimumSize = drawing.Size(676, 600)
        self.Closed += self.closeDialog
        self.SizeChanged += self.OnSizeChanged
        self.LocationChanged += self.OnPositionChanged
        
        ########################################################################
        #Form Icon
        def loadImages():
            path = rc.PlugIns.PlugIn.PathFromName("Tabl_")
            dirName = op.dirname(path)
            try:
                iconPath = op.join(dirName, r'main.ico')
                self.Icon = drawing.Icon(iconPath)
            except:
                print "Could not load mainIcon.ico"

            try:
                addIconPath = op.join(dirName, r'add.ico')
                self.addImage = drawing.Icon(addIconPath)
            except:
                self.addImage = None
                print "Could not load addIcon.ico"

            try:
                removeIconPath = op.join(dirName, r'remove.ico')
                self.removeImage = drawing.Icon(removeIconPath)
            except:
                self.removeImage = None
                print "Could not load removeIcon.ico"

            try:
                updateIconPath = op.join(dirName, r'refresh.ico')
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
            #Create Menu Bar
            def createMenuBar():
                mnuFile = forms.ButtonMenuItem(Text = "File")
                sep1 = forms.SeparatorMenuItem()
                mnuExport = forms.ButtonMenuItem(Text = "Export as...")
                mnuExport.Click += self.exportData
                mnuClose = forms.ButtonMenuItem(Text = "Close")
                mnuClose.Click += self.closeDialog
                mnuNew = forms.ButtonMenuItem(Text = "Clear Tabl")
                mnuNew.Click += self.OnNewTable
                mnuSaveAs = forms.ButtonMenuItem(Text = "Export GUIDs")
                mnuSaveAs.Click += self.OnSaveTable
                mnuOpen = forms.ButtonMenuItem(Text = "Import GUIDs")
                mnuOpen.Click += self.OnOpenTable
                sep2 = forms.SeparatorMenuItem()
                sep3 = forms.SeparatorMenuItem()
                mnuFile.Items.Add(mnuNew)
                mnuFile.Items.Add(sep1)
                mnuFile.Items.Add(mnuExport)
                mnuFile.Items.Add(sep2)
                mnuFile.Items.Add(mnuSaveAs)
                mnuFile.Items.Add(mnuOpen)
                mnuFile.Items.Add(sep3)
                mnuFile.Items.Add(mnuClose)
                
                mnuEdit = forms.ButtonMenuItem(Text = "Edit")
                mnuCopyAll = forms.ButtonMenuItem(Text = "Copy All")
                mnuCopyAll.Click += self.copyToClipboard
                mnuCopySel = forms.ButtonMenuItem(Text = "Copy Selection")
                mnuCopySel.Click += self.copySelectionToClipboard
                mnuEdit.Items.Add(mnuCopyAll)
                mnuEdit.Items.Add(mnuCopySel)
                
                mnuParameters = forms.ButtonMenuItem(Text = "Parameters")
                mnuParamViewAll = forms.ButtonMenuItem(Text = "View All")
                mnuParamViewAll.Click += self.OnViewAll
                mnuParameters.Items.Add(mnuParamViewAll)
                mnuParamHideAll = forms.ButtonMenuItem(Text = "Hide All")
                mnuParamHideAll.Click += self.OnHideAll
                mnuParameters.Items.Add(mnuParamHideAll)
                
                mnuBar = forms.MenuBar(mnuFile, mnuEdit, mnuParameters)
                self.Menu = mnuBar
            
            createMenuBar()
            
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
            try:
                for i, parameter in enumerate(self.settings.parameters):
                    newCol = forms.GridColumn()
                    newCol.Visible = parameter.checkBox.Checked
                    newCol.HeaderText = parameter.col.HeaderText
                    
                    #This textboxcell index is different when columns are reordered. So have to create new one. Otherwise, when added to grid, they are in wrong position.
                    newCol.DataCell = forms.TextBoxCell(parameter.colNum)
                    newCol.DataCell.TextAlignment = parameter.col.DataCell.TextAlignment
                    newCol.Sortable = parameter.col.Sortable
                    newCol.Editable = parameter.col.Editable
                    
                    self.settings.parameters[i].col = newCol
                    self.grid.Columns.Add(newCol)
            except:
                print "Column and Checkbox Setup failed"
            
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
            self.countLbl = forms.Label(Text = "    Selection Error")
            
            ########################################################################
            def createButtons():
                #Button - Add to selection
                self.addSelection = forms.Button()
                self.addSelection.Width = 30
                if self.addImage is None:
                    self.addSelection.Text = " + "
                else:
                    self.addSelection.Image = self.addImage
                self.addSelection.ToolTip = "Add objects to the tabl "
                self.addSelection.Click += self.AddByPicking

                #Button - Update Data
                self.updateSelection = forms.Button()
                if self.updateImage is None:
                    self.updateSelection.Text = "Update"
                else:
                    self.updateSelection.Image = self.updateImage
                self.updateSelection.ToolTip = "Update the Tabl"
                self.updateSelection.Width = 30
                self.updateSelection.Click += self.Regen

                #Button - Remove from selection
                self.removeSelection = forms.Button()
                if self.removeImage is None:
                    self.removeSelection.Text = " - "
                else:
                    self.removeSelection.Image = self.removeImage
                self.removeSelection.Width = 30
                self.removeSelection.ToolTip = "Remove objects from the Tabl"
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
                self.btnExport.Text = "Export as..."
                self.btnExport.ToolTip = "Export Tabl to CSV, HTML, or TXT"
                self.btnExport.Click += self.exportData

                #Button - Close
                self.btnClose = forms.Button()
                self.btnClose.Text = "Close"
                self.btnClose.ToolTip = "Close"
                self.btnClose.Click += self.OnClosedDialog
                
                #Button - Place
                self.btnPlace = forms.Button()
                self.btnPlace.Text = "Place"
                self.btnPlace.ToolTip = "Place Tabl in Rhino"
                self.btnPlace.Click += self.OnPlaceBtnPressed
            createButtons()
            
            ########################################################################
            #Checkbox - Show Units
            self.showUnitsBox = forms.CheckBox()
            self.showUnitsBox.Checked = self.settings.showUnits
            self.showUnitsBox.Text = "Show Units\t"
            self.showUnitsBox.CheckedChanged += self.OnOptionsChanged
            
            #Checkbox - Show Total
            self.showTotalsBox = forms.CheckBox()
            self.showTotalsBox.Checked = self.settings.showTotals
            self.showTotalsBox.Text = "Show Total\t"
            self.showTotalsBox.CheckedChanged += self.OnOptionsChanged
            
            #Checkbox - Show Headers
            self.showHeadersBox = forms.CheckBox()
            self.showHeadersBox.Checked = self.settings.showHeaders
            self.showHeadersBox.Text = "Export Headers\t"
            self.showHeadersBox.CheckedChanged += self.OnOptionsChanged
            
            
            ########################################################################
            def createGroups():
                #Groupbox - Checkboxes - Parameters
                self.parameterGroup = forms.GroupBox(Text = "Properties")
                self.parameterGroup.Padding = drawing.Padding(5)
                self.parameterLayout = forms.DynamicLayout()
                for parameter in self.settings.parameters:
                    self.parameterLayout.AddRow(parameter.checkBox)
                self.parameterLayout.AddRow(None)
                self.parameterGroup.Content = self.parameterLayout
                
                
                
                
                #Groupbox - Checkboxes - Options
                self.optionsGroup = forms.GroupBox(Text = "Options")
                self.optionsGroup.Padding = drawing.Padding(5)
                self.optionsLayout = forms.DynamicLayout()
                self.optionsLayout.AddRow(self.showUnitsBox)
                self.optionsLayout.AddRow(self.showTotalsBox)
                self.optionsLayout.AddRow(self.showHeadersBox)
                self.optionsLayout.AddRow(None)
                self.optionsGroup.Content = self.optionsLayout
            createGroups()
            
            ########################################################################
            #Dynamic Layout - gridLayout
            self.gridGroupBox = forms.GroupBox(Text = "Tabl_")
            self.gridLayout = forms.DynamicLayout()
            self.gridLayout.AddRow(self.grid)
            self.gridLayout.AddRow(None)
            self.gridLayout.AddSeparateRow(self.addSelection, self.removeSelection,self.updateSelection, self.btnSettings, None,self.btnPlace, self.btnCopy, self.btnExport, self.btnClose)
            self.gridGroupBox.Content = self.gridLayout
            self.gridLayout.Spacing = drawing.Size(5, 5)
            
            #Dynamic Layout - paramPanel
            self.paramPanel = forms.DynamicLayout()
            self.paramPanel.AddSeparateRow(self.parameterGroup)
            self.paramPanel.AddSeparateRow(self.optionsGroup)
            self.paramPanel.AddSeparateRow(self.countLbl)
            self.paramPanel.AddSeparateRow(None)
        except:
            print "CreateControls() failed"
    
    #CreateEvents
    def CreateEvents(self):
        rc.Commands.Command.EndCommand += self.OnModifyObject
        sc.doc.ActiveDoc.ModifyObjectAttributes += self.OnModifyObject
    
    def OnSettingsDialog(self, sender, e):
        try:
            settingsDialog = SettingsDialog(self.settings)
            settingsDialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow)
            if settingsDialog.Result:
                self.RegenData()
                self.RegenFormat()
        except:
            print "OnSettingsDialog() failed"
    
    def OnNewTable(self, sender, e):
        try:
            result = rs.MessageBox("Are you sure you want to clear the Tabl?", 1 | 32 | 4096, 'New Tabl')
            if result == 1:
                self.settings.guids = []
                self.Regen()
        except:
            print "New Tabl failed"
    
    def OnSaveTable(self, sender, e):
        try:
            path = rs.SaveFileName('Export GUIDs', "TXT Files (*.txt)|*.txt||")
            
            with open(path, 'w') as filehandle:
                filehandle.writelines("%s\n" % guid for guid in self.settings.guids)            
            
            print "Saved Tabl"
        except Exception, e:
            print e
            
    def OnOpenTable(self, sender, e):
        try:
            path = rs.OpenFileName('Import GUIDs', "TXT Files (*.txt)|*.txt||")
            countErrors = 0
            countSuccess = 0
            with open(path, 'r') as filehandle:
                for line in filehandle:
                    currentPlace = line[:-1]
                    try:
                        if rs.IsObject(currentPlace):
                            self.settings.guids.append(currentPlace)
                            countSuccess += 1
                    except:
                        countErrors += 1
            
            if countSuccess >= 1:
                print "{} GUIDs imported".format(countSuccess)
            if countErrors >= 1:
                print "{} objects cannot be found in this document".format(countErrors)
            
            self.Regen()
        except Exception, e:
            print e
    
    def OnPlaceBtnPressed(self, sender, e):
        try:
            self.Minimize()
            if self.settings.showHeaders:
                tempData = self.VisibleHeadingsList()
                dataToPlace = self.VisibleDataList()
                dataToPlace.insert(0, tempData)
            else:
                dataToPlace = self.VisibleDataList()
            
            placeDialog = PlaceDialog(dataToPlace, self.settings)
            placeDialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow)
            self.BringToFront()
        except:
            print "FAILED"
    
    def OnViewAll(self, sender, e):
        try:
            for parameter in self.settings.parameters:
                parameter.checkBox.Checked = True
        except:
            print "View all failed"
    
    def OnHideAll(self, sender, e):
        try:
            for parameter in self.settings.parameters:
                parameter.checkBox.Checked = False
        except:
            print "Hiding all failed"
    
    #Change selection functions
    def AddByPicking(self, sender, e):
        try:
            self.Minimize()
            rs.LockObjects(self.settings.guids)
            newIDs = rs.GetObjects("Select objects to add to selection", preselect = True)
            rs.UnlockObjects(self.settings.guids)
            if newIDs is None: return
            self.settings.guids += newIDs
            
            #Regen
            self.Regen()
            self.BringToFront()
        except:
            print "AddByPicking() Failed"
            self.BringToFront()

    def RemoveByPicking(self, sender, e):
        try:
            self.Minimize()
            #Get objs to remove
            rs.EnableRedraw(False)
            tempLock = rs.InvertSelectedObjects(rs.SelectObjects(self.settings.guids))
            rs.LockObjects(tempLock)
            rs.EnableRedraw(True)
            items = rs.GetObjects("Select objects to remove from selection", preselect = True)
            rs.UnlockObjects(tempLock)
            if items is None: return
            for item in items:
                if item in self.settings.guids:
                    self.settings.guids.remove(item)
            #Regen
            self.Regen()
            self.BringToFront()
        except:
            print "RemoveByPicking() Failed"
            self.BringToFront()

    def RemoveSelection(self, sender, e):
        #Get objs to remove
        items = list(self.grid.SelectedItems)
        counter = 0
        for item in items:
            try:
                guid = rs.coerceguid(str(item[1]))
                if guid:
                    #Remove from dataStore
                    data = self.grid.DataStore
                    for i, row in enumerate(data):
                        if str(row[1]) == str(guid):
                            data.pop(i)
                            counter += 1
                    self.grid.DataStore = data
                    
                    #Remove from settings
                    if guid in self.settings.guids:
                        self.settings.guids.remove(guid)
            except:
                print "Tabl_ failed to remove an item."
        print "{} items removed from Tabl_".format(counter)
        
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
            for i, obj in enumerate(self.settings.guids):
                if rs.IsObject(obj):
                    if obj not in existingGUIDs:
                        existingGUIDs.append(obj)
                else:
                    count += 1
            if count > 0:
                print "{} objects from previous selection not found.".format(count)
            self.settings.guids = existingGUIDs
        except:
            print "CleanGUIDs() failed"

    def RegenData(self):
        try:
            def RegenDataForObj(num, obj):
                try:
                    isCurve = rs.IsCurve(obj)
                    isSurface = rs.IsSurface(obj)
                    isMesh = rs.IsMesh(obj)
                    if isMesh:
                        meshObj = rs.coercemesh(obj)
                    else:
                        meshObj = None
                except:
                    isCurve = False
                    isSurface = False
                    isMesh = False
                
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
                    if self.settings.colorFormat == 0:
                        color = str(rs.ObjectColor(obj).ToKnownColor())
                        if color == str(0):
                            color = "Other"
                    if self.settings.colorFormat == 1:
                        color = str(rs.ObjectColor(obj).R) + "-" +str(rs.ObjectColor(obj).G) + "-" +str(rs.ObjectColor(obj).B)
                    if self.settings.colorFormat == 2:
                        color = str(rs.ObjectColor(obj).R) + "," +str(rs.ObjectColor(obj).G) + "," +str(rs.ObjectColor(obj).B)
                except:
                    color = "-"
                    print "color failed"

                #Length
                try:
                    if isCurve:
                        length = str(rs.CurveLength(obj) * self.settings.unitScale)
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
                            if rs.IsCurvePlanar(obj):
                                area = str(rs.Area(obj) * (self.settings.unitScale**2))
                    if rs.IsBrep(obj) or isMesh:
                        area = str(rs.Area(obj) * (self.settings.unitScale**2))
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
                            volume = str(rs.SurfaceVolume(obj)[0] * (self.settings.unitScale**3))
                    if isMesh:
                        if rs.IsMeshClosed(obj):
                            volume = str(rs.MeshVolume(obj)[1] * (self.settings.unitScale**3))
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
                    type = rs.ObjectDescription(obj)
                except:
                    type = "Type Error"
                
                #Print Color
                try:
                    if self.settings.colorFormat == 0:
                        p_color = str(rs.ObjectPrintColor(obj).ToKnownColor())
                        if p_color == str(0):
                            p_color = "Other"
                    if self.settings.colorFormat == 1:
                        p_color = str(rs.ObjectPrintColor(obj).R) + "-" +str(rs.ObjectPrintColor(obj).G) + "-" +str(rs.ObjectPrintColor(obj).B)
                    if self.settings.colorFormat == 2:
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
                    linetype = "-"
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
                    elif isMesh:
                       numEdges = str(meshObj.TopologyEdges.Count)
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
                
                #NumFaces
                try:
                    numFaces = ""
                    if isMesh:
                        numFaces = str(rs.MeshFaceCount(obj))
                    elif rs.IsPolysurface(obj):
                        rhobj = rs.coercebrep(obj)
                        numFaces = str(rhobj.Faces.Count)
                    elif isSurface:
                        numFaces = "1"
                except:
                    numFaces = ""
                    print "NumFaces failed"
                
                #Degree
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
                    pt0 = pts[0][0]
                    pt6 = pts[6][0]
                    centerX = str((pt0 + pt6) /2)
                except:
                    centerX = "Failed"
                    print "Center X failed"
                    
                try:
                    pts = rs.BoundingBox(obj)
                    pt0 = pts[0][1]
                    pt6 = pts[6][1]
                    centerY = str((pt0 + pt6) /2)
                except:
                    centerY = "Failed"
                    print "Center Y failed"
                    
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
                    if isMesh:
                        isClosed = str(rs.IsMeshClosed(obj))
                    try:
                        isClosed
                    except:
                        isClosed = ""
                except:
                    isClosed = ""
                    print "isClosed failed"
                
                #Comments
                comments = rs.GetUserText(obj, 'tabl.comment')
                
                if comments is None or len(comments) < 1:
                    rs.SetUserText(obj, 'tabl.comment', ' ')
                    comments = ''
                
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
                numPts,     #13
                numEdges,   #14
                numFaces,   #15
                degree,     #16
                centerX,
                centerY,
                centerZ,
                isPlanar,
                isClosed,
                comments
                ]
                return thisData
            
            self.data = []
            for i, obj in enumerate(self.settings.guids):
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
    def OnOptionsChanged(self, sender, e):
        try:
            self.settings.showUnits = self.showUnitsBox.Checked
            self.settings.showTotals = self.showTotalsBox.Checked
            self.settings.showHeaders = self.showHeadersBox.Checked
            self.Regen()
        except:
            print "Options Change failed"
    
    def OnSelectFromGrid(self, sender, e):
        try:
            rs.UnselectAllObjects()
            selectItems = []
            items = list(self.grid.SelectedItems)
            
            for i, parameter in enumerate(self.settings.parameters):
                if parameter.name == "GUID":
                    GUIDcolNum = i
                    break
            for item in items:
                if rs.IsVisibleInView(str(item[GUIDcolNum])):
                    selectItems.append(str(item[GUIDcolNum]))
                else:
                    print "Object not selected, hidden in current view."
            rs.SelectObjects(selectItems)
        except:
            print "OnSelectFromGrid() failed"

    def OnNameCellChanged(self, sender, e):
        try:
            columnEdited = str(e.GridColumn.HeaderText).split(" ")[0]
            if columnEdited == "Name":
                newName = str(e.Item[3])
                rs.ObjectName(e.Item[1], newName)
            elif columnEdited == "Comments":
                for i, parameter in enumerate(self.settings.parameters):
                    if parameter.name == 'Comments':
                        comment = str(e.Item[i])
                        break
                rs.SetUserText(e.Item[1], 'tabl.comment',comment)
                print "Comment Added"
        except:
            print "OnCellChanged() failed"

    def OnNameChanged(self, sender, e):
        try:
            objects = list(self.grid.SelectedItems)
            newName = rs.StringBox("New Object Name", title = "Rename Objects")
            for obj in objects:
                if len(obj[1]) > 0:
                    rs.ObjectName(obj[1], newName)
            
            self.Regen()
        except:
            print "OnNameChanged() Failed"

    def OnLayerChanged(self, sender, e):
        try:
            objects = list(self.grid.SelectedItems)
            newLayer = rs.GetLayer()
            for obj in objects:
                if len(obj[1]) > 0:
                    rs.ObjectLayer(obj[1], newLayer)
            
            #if self.settings.updateMode == 0:
            self.Regen()
            
        except:
            print "OnLayerChanged() Failed"

    def OnColorChanged(self, sender, e):
        try:
            objects = list(self.grid.SelectedItems)
            newColor = rs.GetColor()
            for obj in objects:
                if len(obj[1]) > 0:
                    rs.ObjectColor(obj[1], newColor)
            
            #if self.settings.updateMode == 0:
            self.Regen()
            
        except:
            print "OnLayerChanged() Failed"

    def OnModifyObject(self, *args):
        try:
            if int(self.settings.updateMode) == 0:
                #If automatic mode selected
                self.Regen()
        except:
            print "OnModifyObject() failed"

    def OnSizeChanged(self, *args):
        self.grid.Height = self.Height-145
        self.settings.dialogWidth = self.Size.Width
        self.settings.dialogHeight = self.Size.Height

    def OnPositionChanged(self, *args):
        self.settings.dialogPosX = self.Location.X
        self.settings.dialogPosY = self.Location.Y
        

    #Get Active
    def VisibleHeadingsList(self):
        currentHeadings = []
        for parameter in self.settings.parameters:
            if parameter.col.Visible:
                currentHeadings.append(parameter.name)
        return currentHeadings
    
    def VisibleDataList(self):
        currentColumnIndexes = []
        
        for i, parameter in enumerate(self.settings.parameters):
            if parameter.checkBox.Checked:
                currentColumnIndexes.append(i)
        
        newDataStore = []
        for row in self.grid.DataStore:
            thisRow = []
            for index in currentColumnIndexes:
                thisRow.append(row[index])
            newDataStore.append(thisRow)
        return newDataStore
    
    def SelectedDataList(self):
        currentColumnIndexes = []
        
        for i, parameter in enumerate(self.settings.parameters):
            if parameter.checkBox.Checked:
                currentColumnIndexes.append(i)
        
        newDataStore = []
        for row in self.grid.SelectedItems:
            thisRow = []
            for index in currentColumnIndexes:
                thisRow.append(row[index])
            newDataStore.append(thisRow)
        return newDataStore    
    
    #SORT
    def sortColumn(self, sender, e):
        try:
            self.CleanGUIDs()
            #Regen Data
            self.RegenData()
            
            self.hideTotalsFunc()
            
            colName = e.Column.HeaderText.split(" ")[0]
            for i, parameter in enumerate(self.settings.parameters):
                if parameter.col == e.Column:
                    self.settings.sortingBy = i
                    parameter.sortDescending = not parameter.sortDescending
                    break
            
            #Headers
            self.AddHeaderArrow()
            #Regen Format
            self.RegenFormat()
        except:
            print "sortColumn() Failed"

    def sortData(self):
        try:
            try:
                self.settings.sortingBy
            except:
                self.settings.sortingBy = 0
            
            state = self.settings.parameters[self.settings.sortingBy].sortDescending
            keys = []
            for row in self.grid.DataStore:
                keyRaw = row[self.settings.sortingBy]
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

    def AddHeaderArrow(self):
        for parameter in self.settings.parameters:
            parameter.col.HeaderText = parameter.col.HeaderText.split(" ")[0]
        
        direction = self.settings.parameters[self.settings.sortingBy].sortDescending
        if direction:
            self.settings.parameters[self.settings.sortingBy].col.HeaderText += " "+ u'\u2191'
        else:
            self.settings.parameters[self.settings.sortingBy].col.HeaderText += " " + u'\u2193'
    
    #CLOSE
    def OnClosedDialog(self, sender, e):
        self.Close()
    
    def closeDialog(self, sender, e):
        try:
            sc.doc.ActiveDoc.ModifyObjectAttributes -= self.OnModifyObject
            rc.Commands.Command.EndCommand -= self.OnModifyObject
        except:
            print "RemoveEvents() failed"
        try:
            sc.sticky['tabl.settings'] = self.settings
            self.Close()
        except:
            print "closeDialog() failed"

    #Functions
    def formatNumbers(self):
        try:
            data = self.grid.DataStore
            for i, row in enumerate(data):
                for j, parameter in enumerate(self.settings.parameters):
                    if parameter.is_float:
                        try:
                            #float(row[j]) #Sometime data in grid is a float, sometimes string, this catches that
                            if len(row[j])>0:
                                if self.settings.commaSep == 0:
                                    data[i][j] = "{0:.{prec}f}".format(float(row[j]), prec = int(self.settings.decPlaces))
                                elif self.settings.commaSep == 1:
                                    data[i][j] = "{0:,.{prec}f}".format(float(row[j]), prec = int(self.settings.decPlaces))
                                elif self.settings.commaSep == 2:
                                    data[i][j] = "{0:,.{prec}f}".format(float(row[j]), prec = int(self.settings.decPlaces)).replace(',', '.')
                                elif self.settings.commaSep == 3:
                                    data[i][j] = "{0:,.{prec}f}".format(float(row[j]), prec = int(self.settings.decPlaces)).replace(',', ' ')
                        except:
                            print "Failed to format number"
            self.grid.DataStore = data
        except:
            print "Showing Thousands Separator Failed"

    def showUnitsFunc(self):
        if self.settings.showUnits:
            if len(self.settings.customUnit) > 0:
                customUnit = True
            else:
                customUnit = False
            
            data = self.grid.DataStore
            try:
                for row in data:
                    for i, parameter in enumerate(self.settings.parameters):
                        try:
                            if len(row[i]) > 0:
                                if parameter.units:
                                    if customUnit:
                                        row[i] += " " + self.settings.customUnit + parameter.superscript
                                    else:
                                        row[i] += " " + parameter.units + parameter.superscript
                        except:
                            pass
            except:
                print "showUnitsFunc() Failed"
            self.grid.DataStore = data

    def hideUnitsFunc(self):
        data = self.grid.DataStore
        try:
            for row in data:
                for i, parameter in enumerate(self.settings.parameters):
                    try:
                        parameter.units
                    except:
                        row[i].split(" ")[0]
        except:
            print "hideUnitsFunc() Failed"
        self.grid.DataStore = data

    def showTotalsFunc(self):
        if self.settings.showTotals:
            try:
                data = self.grid.DataStore
                self.hideUnitsFunc()
                secondLastRow = []
                lastRow = []
                for i in range(len(self.settings.parameters)):
                    secondLastRow.append("")
                    lastRow.append("")
                lastRow[0] = "TOTAL"
                
                for i, parameter in enumerate(self.settings.parameters):
                    if parameter.totalable:
                        total = 0
                        for row in data:
                            if len(row[i]) > 0:
                                try:
                                    total += float(row[i])
                                except:
                                    pass
                        lastRow[i] = str(total)
                data.append(secondLastRow)
                data.append(lastRow)
                self.grid.DataStore = data
            except:
                print "showTotals() failed"

    def hideTotalsFunc(self):
        try:
            tempData = self.grid.DataStore
            if tempData[-1][0] == "TOTAL":
                del tempData[-1]
                del tempData[-1]
                self.grid.DataStore = tempData
            else:
                return
        except:
            print "hideTotalsFunc() failed"

    def recountLabel(self):
        self.countLbl.Text = "Selected: {} objects".format(len(self.settings.guids))

    #I/O Functions
    def copyToClipboard(self, sender, e):
        try:
            string = self.DataStoreToHTML()
            rs.ClipboardText(string)
            print "Copied Tabl to clipboard"
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
            fileName = rs.SaveFileName("Save Tabl", filter = "CSV Files (*.csv)|*.csv|HTML Files (*.html)|*.html|TXT Files (*.txt)|*.txt||")
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
        
        if self.settings.showHeaders:
            allHeadings = self.VisibleHeadingsList()
            for heading in allHeadings:
                itemLen = len(str(heading))
                if itemLen >= 8:
                    numTabs = 1
                else:
                    numTabs = 2
                string += str(heading) + (seperator * numTabs)
            string += "\n"
        
        allData = self.VisibleDataList()
        
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
        if self.settings.showHeaders:
            allHeadings = self.VisibleHeadingsList()
            for heading in allHeadings:
                string += str(heading) + ","
            string += "\n"

        allData = self.VisibleDataList()

        for row in allData:
            for item in row:
                if str(item) == "None":
                    string += ","
                else:
                    string +=  '"' + str(item) + '",'
            string += "\n"
        return string

    def DataStoreToHTML(self, *args):
        string = "<html><head><style>"
        string += "body {color: dimgray;}"
        string += "table, th, td{border-collapse: collapse; border: 1px solid black;padding: 10px;}"
        string += "</style></head><body><table>"
        
        if self.settings.showHeaders:
            allHeadings = self.VisibleHeadingsList()
            string += "<tr>"
            for heading in allHeadings:
                string += "<th>" + str(heading) + "</th>"
            string += "</tr>"
        
        if len(args) == 0:
            allData = self.VisibleDataList()
        else:
            allData = self.SelectedDataList()
        
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