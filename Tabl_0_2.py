import Rhino
import Eto.Drawing as drawing
import Eto.Forms as forms
import rhinoscriptsyntax as rs
import scriptcontext as sc
import os.path
import System.Drawing.Color

class PropertiesDialog():
    def __init__(self):
        self.ApplyBoo = False
        
        self.paramsMaster = [
        ['#', 'Rhino', '0', '0' ],
        ['GUID', 'Rhino', '1', '1'],
        ['Type', 'Rhino', '2', '2'],
        ['Name', 'Rhino', '3', '3'],
        ['Layer', 'Rhino', '4', '4'],
        ['Display Color', 'Rhino', '5', '5'],
        ['Linetype', 'Rhino', '6', '6'],
        ['Print Color', 'Rhino', '7', '7'],
        ['Print Width', 'Rhino', '8', '8'],
        ['Render Material', 'Rhino', '9', '9'],
        ['Length', 'Rhino', '10', '10'],
        ['Area', 'Rhino', '11', '11'],
        ['Volume', 'Rhino', '12', '12'],
        ['IsPlanar', 'Rhino', '16', '16'],
        ['IsClosed', 'Rhino', '17', '17']
        ]
        
        self.displayParams = []
        
        self.Title = "Parameter Manager"
        self.grid_Master = forms.GridView()
        self.grid_Master.ShowHeader = True
        self.grid_Master.DataStore = self.paramsMaster
        self.grid_Master.CellEdited += self.OnCellEdited
        self.grid_Master.Height = 200
        self.grid_Master.Width = 300
        self.grid_Master.AllowMultipleSelection = True
        self.nameCol = forms.GridColumn()
        self.nameCol.HeaderText = "Name\t\t"
        self.nameCol.DataCell = forms.TextBoxCell(0)
        self.nameCol.Editable = True
        self.grid_Master.Columns.Add(self.nameCol)
        self.formulaCol = forms.GridColumn()
        self.formulaCol.HeaderText = "Formula\t\t"
        self.formulaCol.DataCell = forms.TextBoxCell(2)
        self.formulaCol.Editable = True
        self.grid_Master.Columns.Add(self.formulaCol)
        
        self.grid_Display = forms.GridView()
        self.grid_Display.ShowHeader = True
        self.grid_Display.DataStore = []
        self.grid_Display.Height = 200
        self.grid_Display.Width = 300
        self.grid_Display.AllowMultipleSelection = True
        self.nameColDisplayed = forms.GridColumn()
        self.nameColDisplayed.HeaderText = "Name\t\t"
        self.nameColDisplayed.DataCell = forms.TextBoxCell(0)
        self.nameColDisplayed.Editable = True
        self.grid_Display.Columns.Add(self.nameColDisplayed)
        self.formulaColDisplayed = forms.GridColumn()
        self.formulaColDisplayed.HeaderText = "Formula\t\t"
        self.formulaColDisplayed.DataCell = forms.TextBoxCell(2)
        self.formulaColDisplayed.Editable = True
        self.grid_Display.Columns.Add(self.formulaColDisplayed)
        
        self.btnToRight = forms.Button(Text = " > ")
        self.btnToRight.Size = drawing.Size(30,30)
        self.btnToRight.Click += self.OnToRightPressed
        self.btnToLeft = forms.Button(Text = " < ")
        self.btnToLeft.Size = drawing.Size(30,30)
        self.btnToLeft.Click += self.OnToLeftPressed
        
        self.btnUp = forms.Button(Text = " ^ ")
        self.btnUp.Click += self.OnUpPressed
        self.btnUp.Size = drawing.Size(30,30)
        self.btnDown = forms.Button(Text = " v ")
        self.btnDown.Click += self.OnDownPressed
        self.btnDown.Size = drawing.Size(30,30)
        
        self.btnNewParam = forms.Button(Text = "New")
        self.btnNewParam.Size = drawing.Size(90,30)
        self.btnNewParam.Click += self.OnNewParam
        self.btnRemoveParam = forms.Button(Text = "Remove")
        self.btnRemoveParam.Size = drawing.Size(90,30)
        self.btnRemoveParam.Click += self.OnRemoveParam
        
        layoutColumn = forms.DynamicLayout()
        layoutColumn.AddSeparateRow(self.btnUp)
        layoutColumn.AddSeparateRow(self.btnDown)
        layoutColumn.AddSeparateRow(None)

        self.layout = forms.DynamicLayout()
        self.layout.AddSeparateRow(self.grid_Master, self.grid_Display, None ,layoutColumn)
        self.layout.AddSeparateRow(self.btnNewParam, self.btnRemoveParam, self.btnToLeft, self.btnToRight, None)
        self.layout.AddSeparateRow(None)
        self.Content = self.layout
    
    def OnToRightPressed(self, sender, e):
        try:
            colsToMove = list(self.grid_Master.SelectedItems)
            tempMasterData = list(self.grid_Master.DataStore)
            
            for colToMove in colsToMove:
                #Move the top item if none selected
                if colToMove is None:
                    try:
                        colToMove = self.grid_Master.DataStore[0]
                    except: return
                #remove from old grid
                tempMasterData.remove(colToMove)
            
            self.grid_Master.DataStore = tempMasterData
            
            #Add columns to display grid
            tempDisplayData = list(self.grid_Display.DataStore)
            for colToMove in colsToMove:
                tempDisplayData.append(colToMove)
            self.grid_Display.DataStore = tempDisplayData
            
            #Save to sticky
            sc.sticky['grid_Display.DataStore'] = self.grid_Display.DataStore
        except:
            print "OnToRightPressed failed"

    def OnToLeftPressed(self, sender, e):
        try:
            colsToMove = list(self.grid_Display.SelectedItems)
            tempDisplayData = list(self.grid_Display.DataStore)
            for colToMove in colsToMove:
                if colToMove is None:
                    try:
                        colToMove = self.grid_Display.DataStore[0]
                    except: return
            
                #Remove from display grid
                tempDisplayData.remove(colToMove)
            self.grid_Display.DataStore = tempDisplayData
            
            #Fill initial visInMaster
            self.visInMaster = []
            for i in range(len(self.paramsMaster)):
                self.visInMaster.append(False)
            
            #Check if visInMaster
            newIndicies = []
            for i in range(len(self.paramsMaster)):
                for each in self.grid_Master.DataStore:
                    if self.paramsMaster[i][0] == each[0]:
                        self.visInMaster[i] = True
                        break
                for colToMove in colsToMove:
                    if colToMove[0] == self.paramsMaster[i][0]:
                        newIndicies.append(i)
            
            #Change the item thats coming back
            for each in newIndicies:
                self.visInMaster[each] = True
            
            #Apply visInMaster as mask
            newMasterData = []
            for i, each in enumerate(self.visInMaster):
                if each:
                    newMasterData.append(self.paramsMaster[i])
            self.grid_Master.DataStore = newMasterData
            
            #Save to sticky
            sc.sticky['grid_Display.DataStore'] = self.grid_Display.DataStore
        except:
            print "OnToLeftPressed failed"
    
    def OnUpPressed(self, sender, e):
        try:
            if self.grid_Display.SelectedItem is None:
                return
            tempDisplayData = list(self.grid_Display.DataStore)
            for i, each in enumerate(tempDisplayData):
                if each[0] == self.grid_Display.SelectedItem[0]:
                    selectedRow = i
                    break
            newPos = selectedRow - 1
            if newPos < 0:
                newPos = len(tempDisplayData)
            tempDisplayData.insert(newPos, tempDisplayData.pop(selectedRow))
            self.grid_Display.DataStore = tempDisplayData
        except:
            print "OnUpPressed failed"
    
    def OnDownPressed(self, sender, e):
        try:
            if self.grid_Display.SelectedItem is None:
                return
            tempDisplayData = list(self.grid_Display.DataStore)
            for i, each in enumerate(tempDisplayData):
                if each[0] == self.grid_Display.SelectedItem[0]:
                    selectedRow = i
                    break
            newPos = selectedRow + 1
            if newPos > len(tempDisplayData)-1:
                newPos = 0
            tempDisplayData.insert(newPos, tempDisplayData.pop(selectedRow))
            self.grid_Display.DataStore = tempDisplayData
        except:
            print "OnDownPressed failed"
    
    def OnNewParam(self, sender, e):
        try:
            existingData = self.grid_Master.DataStore
            newParamItem = ['', 'User Text', '', '']
            existingData.append(newParamItem)
            self.grid_Master.DataStore = existingData
            row = int(len(existingData))
            self.grid_Master.BeginEdit(row-1, 0)
        except:
            print "OnNewParam failed"
    
    def OnRemoveParam(self, sender, e):
        try:
            items = list(self.grid_Master.SelectedItems)
            if items is None:
                return
            tempMasterData = list(self.grid_Master.DataStore)
            
            for item in items:
                if item[1] == "User Text":
                    tempMasterData.remove(item)
                    self.paramsMaster.remove(item)
            
            self.grid_Master.DataStore = tempMasterData
        except:
            print "OnRemoveParam failed"
    
    def OnCellEdited(self, sender, e):
        try:
            existingData = list(self.grid_Master.DataStore)
            existingData.pop(e.Row)
            row = int(e.Row)
            col = int(e.Column)
            for each in existingData:
                if e.Item[0] == each[0]:
                    print "Warning: Property name already exists"
                    break
        except:
            print "OnCellEdited failed"

class WebBrowser(forms.Dialog):
    def __init__(self, path):
        self.Title = "User Manual"
        webview = forms.WebView()
        webview.Size = drawing.Size(900,600)
        webview.Url = System.Uri(path)
        self.Content = webview

class SettingsDialog(forms.Dialog):
    def __init__(self, tempColorFormat):
        #Variables
        self.applyBool = False
        self.initColorFormat = tempColorFormat
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
        
        self.showCommas = forms.CheckBox(Text = "Thousands Separator")
        try:
            self.showCommas.Checked = commaSep
        except:
            print "showCommas failed"

        self.btnApply = forms.Button()
        self.btnApply.Text = "OK"
        self.btnApply.Click += self.OnApplySettings

        self.seperatorDropDown = forms.DropDown()
        self.seperatorDropDown.DataStore = ['None','Comma\t","', 'Dot\t"."', 'Space\t" "']
        global commaSep
        self.seperatorDropDown.SelectedIndex = commaSep

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
            self.colorRadioBtn.SelectedIndex = self.initColorFormat
        except:
            print "colorRadioBtn failed"
        
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
            global decPlaces
            decPlaces = self.numericDecPlaces.Value
            global commaSep
            commaSep = self.seperatorDropDown.SelectedIndex
            sc.sticky['decPlaces'] = int(decPlaces)
            sc.sticky['commaSep'] = int(self.seperatorDropDown.SelectedIndex)
            sc.sticky['colorFormat'] = int(self.colorRadioBtn.SelectedIndex)
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

class PlaceDialog(forms.Dialog):
    def __init__(self, currentData):
        #Variables
        if 'bufferSize' in sc.sticky:
            self.bufferSize = sc.sticky['bufferSize']
        else:
            self.bufferSize = 2 
        if 'placeType' in sc.sticky:
            self.placeType = sc.sticky['placeType']
        else:
            self.placeType = 0 
        
        self.Title = "Place Settings"
        #self.ClientSize = drawing.Size(300, 200)
        self.Padding = drawing.Padding(5)
        self.Resizable = False
        self.CreateLayout()
        self.data = currentData
    
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
        print "Creating Controls"
        self.PlaceBtn = forms.Button()
        self.PlaceBtn.Text = "Place"
        self.PlaceBtn.ToolTip = "Place table inside Rhino"
        self.PlaceBtn.Click += self.OnPlace

        self.btnCancel = forms.Button()
        self.btnCancel.Text = "Cancel"
        self.btnCancel.Click += self.OnCancelSettings
        
        self.bufferUpDownLbl = forms.Label(Text = " Buffer Size")
        self.bufferUpDown = forms.NumericUpDown()
        self.bufferUpDown.DecimalPlaces = 0
        self.bufferUpDown.MinValue = 0
        self.bufferUpDown.Value = self.bufferSize
        
        self.PlaceTypeRadio = forms.RadioButtonList()
        self.PlaceTypeRadio.DataStore = [
        "Fit Columns To Data", 
        "Fit Columns To Table Width",
        "Fixed Table Width (Divide Even)", 
        "Fixed Column Width"]
        self.PlaceTypeRadio.Orientation = forms.Orientation.Vertical
        self.PlaceTypeRadio.SelectedIndex = self.placeType

    def OnCancelSettings(self, sender, e):
        try:
            print "Cancelled"
            self.Close()
        except:
            print "Failed to cancel settings"

    #Place Table
    def OnPlace(self, sender, e):
        try:
            sc.sticky['bufferSize'] = int(self.bufferUpDown.Value)
            sc.sticky['placeType'] = int(self.PlaceTypeRadio.SelectedIndex)
            self.Close()
            self.PlaceTable(self.PlaceTypeRadio.SelectedIndex)
        except:
            print "PlaceTable failed"
    
    def PlaceTable(self, type):
        """
        Places table according to type specified.
        Type 0 = Fit Columns To Data
        Type 1 = Fit Columns To Table Width
        Type 2 = Fixed Table Width
        Type 3 = Fixed Column Width
        Type 4 = Fill Rectangle
        """
        
        def placeTable_FitWidth(dataLen, bufferSize):
            minColWidths = []
            transposed = zip(*dataLen)
            for item in transposed:
                minColWidth = 0
                for param in item:
                    if param > minColWidth:
                        minColWidth = param
                minColWidths.append(minColWidth + (bufferSize*2))
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
                minColWidths.append(minColWidth + (bufferSize*2))
            count = 0
            for width in minColWidths:
                count += width
            print count
            print tableWidth
            for width in minColWidths:
                tempWidth = mapFromTo(width, 0, count, 0, tableWidth)
                colWidths.append(tempWidth)
            return colWidths
        
        def placeTable_FixedColWidth(fixedWidth, dataLen):
            fixedWidths = []
            transposed = zip(*dataLen)
            for item in transposed:
                fixedWidths.append(fixedWidth)
            return fixedWidths
        
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
        
        
        def GetRectangleLocation(width, height):
            line_color = System.Drawing.Color.LightGray
        
            def GetPointDynamicDrawFunc( sender, args ):
                xLoc = args.CurrentPoint[0]
                yLoc = args.CurrentPoint[1]
                pt0 = Rhino.Geometry.Point3d(xLoc,yLoc,0)
                pt1 = Rhino.Geometry.Point3d(xLoc+width,yLoc,0)
                pt2 = Rhino.Geometry.Point3d(xLoc+width,yLoc-height,0)
                pt3 = Rhino.Geometry.Point3d(xLoc,yLoc-height,0)
                allPts = [pt0, pt1, pt2, pt3, pt0]
                args.Display.DrawPolyline(allPts, line_color, 1)
        
            gp = Rhino.Input.Custom.GetPoint()
            gp.SetCommandPrompt("Choose point to place table")
            gp.DynamicDraw += GetPointDynamicDrawFunc
            gp.Get()
            if (gp.CommandResult() == Rhino.Commands.Result.Success):
                pt = gp.Point()
                return pt
        
        #Variables
        textSize = sc.doc.ActiveDoc.DimStyles.Current.TextHeight
        bufferSize = self.bufferUpDown.Value
        
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
        
        if len(cleanData) == 0:
            print "No data to place"
            return
        
        #get dataLen
        dataLen = []
        for item in cleanData:
            itemLen = []
            for param in item:
                text = str(param)
                plane = Rhino.Geometry.Plane.WorldXY
                dimStyle = sc.doc.ActiveDoc.DimStyles.CurrentDimensionStyle
                textObj = Rhino.Geometry.TextEntity.Create(text, plane, dimStyle, False,0,0)
                length = textObj.TextModelWidth
                itemLen.append(length)
            dataLen.append(itemLen)
        
        #get minColWidth
        if type == 0:
            colWidths = placeTable_FitWidth(dataLen, bufferSize)
        if type == 1:
            tableWidth = rs.GetReal("Table Width", number = 100, minimum = 1)
            if tableWidth is None: return
            colWidths = placeTable_FitWidthToTable(tableWidth, dataLen, bufferSize)
        if type == 2:
            tableWidth = rs.GetReal("Table Width", number = 100, minimum = 1)
            if tableWidth is None: return
            colWidths = placeTable_FixedTableWidth(tableWidth, dataLen)
        if type == 3:
            fixedWidth = rs.GetReal("Column Width", number = 30, minimum = 1)
            if fixedWidth is None: return
            colWidths = placeTable_FixedColWidth(fixedWidth, dataLen)
        
        #get rowHeight
        rowHeight = textSize + (bufferSize*2)
        
        #Get overall width, height
        tableWidth = 0
        for col in colWidths:
            tableWidth += col
        
        tableHeight = 0
        for each in cleanData:
            tableHeight += rowHeight
        
        #getFramePts & textPts
        placePt = GetRectangleLocation(tableWidth,tableHeight)
        if placePt is None: return
        xLoc = placePt[0]
        yLoc = placePt[1]
        zLoc = placePt[2]
        framePts = []
        textPts = []
        for item in cleanData:
            tempFramePts = []
            tempTextPts = []
            for i, param in enumerate(item):
                tempFramePts.append([xLoc, yLoc, zLoc])
                tempTextPts.append([xLoc + bufferSize, yLoc - rowHeight+bufferSize, zLoc])
                xLoc += colWidths[i]
            yLoc -= rowHeight
            xLoc = placePt[0]
            framePts.append(tempFramePts)
            textPts.append(tempTextPts)
        
        #Add Text and Rectangles
        allText = []
        allRect = []
        rs.EnableRedraw(False)
        sn = sc.doc.BeginUndoRecord("Place Table")
        for i, item in enumerate(cleanData):
            for j, param in enumerate(item):
                allText.append(rs.AddText(str(param), textPts[i][j], textSize))
                plane = Rhino.Geometry.Plane.WorldXY
                plane.Origin = rs.coerce3dpoint(framePts[i][j])
                width = colWidths[j]
                height = rowHeight
                allRect.append(rs.AddRectangle(plane, width, -height))
        if (sn > 0):
            sc.doc.EndUndoRecord(sn)
        rs.EnableRedraw(True)
        return allText, allRect

class Tabl_(forms.Form):
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
        print "Initializing"
        
        #Properties tab
        self.propDialog = PropertiesDialog()
        
        #Variables
        self.activeHeadingIndex = []
        self.rows = []
        
        if 'grid_Display.DataStore' in sc.sticky:
            self.propDialog.grid_Display.DataStore = sc.sticky['grid_Display.DataStore']
        else:
            self.propDialog.grid_Display.DataStore = [['#', 'Rhino', '0', '0'], ['Name', 'Rhino', '3', '3']]
        ########################################################################
        #Form Setup
        self.Title = "Tabl_"
        self.Resizable = True
        if 'dialogPos' in sc.sticky:
            self.Location = sc.sticky['dialogPos']
        if 'dialogSize' in sc.sticky:
            self.Size = sc.sticky['dialogSize']
        else:
            self.Size = drawing.Size(300,300)
        #self.ClientSize = drawing.Size(700,700)
        self.MinimumSize = drawing.Size(650, 450)
        self.Closed += self.closeDialog
        self.SizeChanged += self.OnSizeChanged
        ########################################################################

        
        #Form Icon
        def loadImages():
            pixelformat = drawing.PixelFormat.Format32bppRgba
            path = Rhino.PlugIns.PlugIn.PathFromName("Tablr")
            dirName = os.path.dirname(path)
            print dirName
            try:
                #Failed
                iconPath = dirName + r"\mainIcon.ico"
                self.Icon = drawing.Icon(iconPath)
                print "Loaded iconGrid.ico"
            except:
                print "Could not load iconGrid.ico"

            try:
                #Shrinks
                addIconPath = dirName + r"\addIcon.ico"
                self.addImage = drawing.Icon(addIconPath)
                print "Loaded addIcon.ico"
            except:
                self.addImage = None
                print "Could not load addIcon.ico"

            try:
                #Good
                removeIconPath = dirName + r"\removeIcon.ico"
                self.removeImage = drawing.Icon(removeIconPath)
                
                print "Loaded removeIcon.ico"
            except:
                self.removeImage = None
                print "Could not load removeIcon.ico"

            try:
                #good - no transparent
                updateIconPath = dirName + r"\refreshIcon.ico"
                self.updateImage = drawing.Icon(updateIconPath)
                print self.updateImage
                print "Loaded updateIcon.ico"
            except:
                self.updateImage = None
                print "Could not load updateIcon.ico"
        loadImages()

    #Create Layout
    def CreateLayout(self):
        print "Creating Layout"
        #Create Grid, Buttons, Checkboxes, Dropdowns
        self.CreateControls()
        ########################################################################
        #Layout
        try:
            #Dynamic Layout - buttonLayout
            buttonLayout = forms.DynamicLayout()
            buttonLayout.AddRow(self.countLbl)
            buttonLayout.AddSeparateRow(self.addSelection, self.removeSelection,self.updateSelection, None, self.btnSettings, self.btnPlace, self.btnCopy, self.btnExport, self.btnClose)
            
            layout = forms.DynamicLayout()
            layout.AddRow(self.grid)
            layout.AddRow(buttonLayout)
            layout.AddSeparateRow(None)
            
            
            #Tabs
            tabPage1 = forms.TabPage()
            tabPage1.Text = "Table"
            tabPage1.Content = layout
            
            tabPage2 = forms.TabPage()
            tabPage2.Text = "Property Manager"
            tabPage2.Content = self.propDialog.Content
            
            self.tabCtrl = forms.TabControl()
            self.tabCtrl.Pages.Add(tabPage1)  
            self.tabCtrl.Pages.Add(tabPage2)  
            self.tabCtrl.SelectedIndexChanged += self.OnTabChanged
            self.Content = self.tabCtrl
        except:
            print "CreateLayout() failed"

    def CreateControls(self):
        print "Creating Controls"
        try:
            ####################################################################
            #Create Menu Bar
            def createMenuBar():
                mnuFile = forms.ButtonMenuItem(Text = "File")
                mnuExport = forms.ButtonMenuItem(Text = "Export")
                mnuExport.Click += self.exportData
                mnuClose = forms.ButtonMenuItem(Text = "Close")
                mnuClose.Click += self.closeDialog
                mnuNew = forms.ButtonMenuItem(Text = "New")
                mnuNew.Click += self.OnNewTable
                mnuFile.Items.Add(mnuNew)
                mnuFile.Items.Add(mnuExport)
                mnuFile.Items.Add(mnuClose)
                
                mnuEdit = forms.ButtonMenuItem(Text = "Edit")
                mnuCopyAll = forms.ButtonMenuItem(Text = "Copy All")
                mnuCopyAll.Click += self.copyToClipboard
                mnuCopySel = forms.ButtonMenuItem(Text = "Copy Selection")
                mnuCopySel.Click += self.copySelectionToClipboard
                mnuEdit.Items.Add(mnuCopyAll)
                mnuEdit.Items.Add(mnuCopySel)
                
                mnuTest = forms.ButtonMenuItem(Text = "Test")
                mnuTest.Click += self.OnTest
                mnuEdit.Items.Add(mnuTest)
                
                mnuHelp = forms.ButtonMenuItem(Text = "Help")
                mnuTutorial = forms.ButtonMenuItem(Text = "User Manual")
                mnuTutorial.Click += self.OnTutorialsClick
                mnuVersion = forms.ButtonMenuItem()
                global version
                mnuVersion.Text = str(version)
                mnuHelp.Items.Add(mnuTutorial)
                mnuHelp.Items.Add(mnuVersion)
                
                mnuBar = forms.MenuBar(mnuFile, mnuEdit, mnuHelp)
                self.Menu = mnuBar
            createMenuBar()
            ########################################################################
            #GridView - Spreadsheet
            def createGrid():
                self.grid = forms.GridView()
                self.grid.BackgroundColor = drawing.Colors.LightGrey
                self.grid.AllowColumnReordering = True
                self.grid.GridLines = forms.GridLines.Both
                self.grid.Border = forms.BorderType.Line
                self.grid.AllowMultipleSelection = True
                #self.grid.Height = 50
                #self.grid.Size = drawing.Size(600,500)
                self.grid.CellEdited += self.OnNameCellChanged
                self.grid.ColumnHeaderClick += self.sortColumn
            createGrid()
            ########################################################################
            #Context menu
            def createContextMenu():
                self.changeSelectionName = forms.ButtonMenuItem()
                self.changeSelectionName.Text = "Change Object Name"
                self.changeSelectionName.Click += self.OnNameChanged

                self.changeSelectionLayer = forms.ButtonMenuItem()
                self.changeSelectionLayer.Text = "Change Object Layer"
                self.changeSelectionLayer.Click += self.OnLayerChanged

                self.changeSelectionColor = forms.ButtonMenuItem()
                self.changeSelectionColor.Text = "Change Selection Color"
                self.changeSelectionColor.Click += self.OnColorChanged

                self.addItem = forms.ButtonMenuItem()
                self.addItem.Text = "Add Objects"
                self.addItem.Image = self.addImage
                self.addItem.Click += self.AddByPicking

                self.deleteRow = forms.ButtonMenuItem()
                self.deleteRow.Text = "Delete"
                self.deleteRow.Image = self.removeImage
                self.deleteRow.Click += self.RemoveSelection

                self.selectItem = forms.ButtonMenuItem()
                self.selectItem.Text = "Select Objects"
                self.selectItem.Click += self.OnSelectFromGrid

                self.seperator1 = forms.SeparatorMenuItem()
                self.seperator2 = forms.SeparatorMenuItem()
                self.seperator3 = forms.SeparatorMenuItem()
                self.seperator4 = forms.SeparatorMenuItem()

                self.copyItems = forms.ButtonMenuItem()
                self.copyItems.Text = "Copy All"
                self.copyItems.Click += self.copyToClipboard

                self.copySelection = forms.ButtonMenuItem()
                self.copySelection.Text = "Copy Selection"
                self.copySelection.Click += self.copySelectionToClipboard

                self.insertRow = forms.ButtonMenuItem()
                self.insertRow.Text = "Insert Row"
                self.insertRow.Click += self.OnInsertRow
                
                self.moveRowUp = forms.ButtonMenuItem()
                self.moveRowUp.Text = "Move Row Up"
                self.moveRowUp.Click += self.OnMoveRowUp
                
                self.moveRowDown = forms.ButtonMenuItem()
                self.moveRowDown.Text = "Move Row Down"
                self.moveRowDown.Click += self.OnMoveRowDown
                
                self.contextMenu = forms.ContextMenu([self.changeSelectionName, self.changeSelectionLayer, self.changeSelectionColor, self.seperator3, self.addItem, self.deleteRow, self.seperator1, self.selectItem, self.seperator4, self.insertRow, self.moveRowUp, self.moveRowDown, self.seperator2, self.copyItems, self.copySelection])
                self.grid.ContextMenu = self.contextMenu
            createContextMenu()
            ########################################################################
            self.countLbl = forms.Label(Text = "Selection Error")
            ########################################################################
            #Columns
            self.CreateColumnsFromProperties(self.propDialog.grid_Display.DataStore)
            ########################################################################
            #Lists
            self.SetupLists()
            ########################################################################
            def createButtons():
                #Button - Add to selection
                self.addSelection = forms.Button()
                if self.addImage is None:
                    self.addSelection.Text = " + "
                else:
                    self.addSelection.Image = self.addImage
                self.addSelection.ToolTip = "Add objects to the table"
                self.addSelection.Width = 30
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

                #Button - Place
                self.btnPlace = forms.Button()
                self.btnPlace.Text = "Place"
                self.btnPlace.ToolTip = "Place table inside Rhino"
                self.btnPlace.Click += self.OnPlaceDialog

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
        except:
            print "CreateControls() failed"

    #Setup the lists
    def SetupLists(self):
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
        print "Checking sticky"
        try:
            self.CheckStickyForSettings()
            self.GetObjsFromSticky()
        except:
            print "CheckSticky() failed"

    def CheckStickyForSettings(self):
        if 'colorFormat' in sc.sticky:
            self.colorFormat = sc.sticky['colorFormat']
        else:
            self.colorFormat = 1

    def SetObjs2Sticky(self):
        sc.sticky["self.rows"] = self.rows

    def GetObjsFromSticky(self):
        try:
            self.rows = sc.sticky["self.rows"]
        except:
            self.rows = []

    #CreateEvents
    def CreateEvents(self):
        print "Creating Events"
        #Rhino.RhinoApp.EscapeKeyPressed +=
        Rhino.Commands.Command.EndCommand += self.OnModifyObject
        sc.doc.ActiveDoc.ModifyObjectAttributes += self.OnModifyObject
        #sc.doc.ActiveDoc.
        #Rhino.RhinoDoc.DeleteRhinoObject += self.OnDeleteRhinoObject
        #Rhino.RhinoDoc.ModifyObjectAttributes += self.OnModifyObject

    def RemoveEvents(self):
        #Rhino.RhinoDoc.DeleteRhinoObject -= self.OnDeleteRhinoObject
        sc.doc.ActiveDoc.ModifyObjectAttributes -= self.OnModifyObject
        Rhino.Commands.Command.EndCommand -= self.OnModifyObject
        print "Events removed"

    def OnSettingsDialog(self, sender, e):
        try:
            settingsDialog = SettingsDialog(self.colorFormat)
            settingsDialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow)
            if settingsDialog.applyBool:
                self.colorFormat = settingsDialog.colorRadioBtn.SelectedIndex
                self.Regen()
        except:
            print "OnSettingsDialog FAILED"
    
    def OnPlaceDialog(self, sender, e):
        try:
            self.Minimize()
            if self.showHeaders.Checked:
                tempData = self.activeHeadingsList()
                print "activeHeadingsList:"
                print tempData
                dataToPlace = self.activeDataList()
                print "activeDataList:"
                print dataToPlace
                dataToPlace.insert(0, tempData)
            else:
                dataToPlace = self.activeDataList()
            
            placeDialog = PlaceDialog(dataToPlace)
            placeDialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow)
            self.BringToFront()
        except:
            print "OnPlaceDialog Failed"
    
    def OnInsertRow(self, sender, e):
        try:
            index = list(self.grid.SelectedRows)[0]
            tempDataStore = self.grid.DataStore
            numProps = len(tempDataStore[0])
            blankRow = []
            for i in range(numProps):
                blankRow.append("")
            tempDataStore.insert(index, blankRow)
            self.rows.insert(index, "")
            self.grid.DataStore = tempDataStore
            self.Regen()
        except:
            print "OnInsertRow failed"
    
    def OnMoveRowUp(self, sender, e):
        try:
            oldData = self.grid.DataStore
            oldRows = list(self.rows)
            indicies = list(self.grid.SelectedRows)
            indicies.sort(key = int)
            for index in indicies:
                if index > 0:
                    oldData.insert(index-1, oldData.pop(index))
                    self.rows.insert(index-1, self.rows.pop(index))
            self.grid.DataStore = oldData
            self.Regen()
        except:
            print "OnMoveRowUp Failed"
    
    def OnMoveRowDown(self, sender, e):
        try:
            oldData = self.grid.DataStore
            indicies = list(self.grid.SelectedRows)
            indicies.sort(key = int)
            indicies.reverse()
            for index in indicies:
                if index < len(oldData):
                    oldData.insert(index+1, oldData.pop(index))
                    self.rows.insert(index+1, self.rows.pop(index))
            self.grid.DataStore = oldData
            self.Regen()
        except:
            print "OnMoveRowDown Failed"
    
    def OnNewTable(self, sender, e):
        try:
            rc = rs.MessageBox("Are you sure you want to clear all data?", 4, "Tabl_")
            if rc is None: return
            if rc == 6:
                self.grid.DataStore = []
                self.rows = []
                self.Regen()
                #self.recountLabel()
        except:
            print "New Table failed"
    
    def OnTutorialsClick(self, sender, e):
        try:
            tutorialFile = r"https://canvasjs.com/html5-javascript-pie-chart/"
            browser = WebBrowser(tutorialFile)
            browser.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow)
        except:
            print "OnTutorialsClick failed"
    
    def OnTabChanged(self, sender, e):
        try:
            if self.tabCtrl.SelectedIndex == 0:
                self.OnTabChangedToTable()
            if self.tabCtrl.SelectedIndex == 1:
                self.OnTabChangedToPropManager()
            #print self.propDialog.grid_Display.DataStore
        except:
            print "OnTabChanged Failed\n***************"

    def OnTabChangedToTable(self):
        try:
            print "Table Tab"
            #Empty self.grid
            self.grid.Columns.Clear()
            self.grid.DataStore = []
            
            self.CreateColumnsFromProperties(self.propDialog.grid_Display.DataStore)
            self.Regen()
        except:
            print "OnTabChangedToTable failed"

    def CreateColumnsFromProperties(self, properties):
        self.activeHeadingIndex = []
        self.activePropertiesFunc = []
        for i, property in enumerate(properties):
            tempCol = forms.GridColumn()
            tempCol.HeaderText = property[0] + " \t"
            tempCol.DataCell = forms.TextBoxCell(i)
            tempCol.Sortable = True
            self.activeHeadingIndex.append(property[2])
            self.activePropertiesFunc.append(property[2])
            if property[1] == "User Text":
                tempCol.Editable = True
            self.grid.Columns.Add(tempCol)

    def OnTabChangedToPropManager(self):
        tempMasterData = list(self.propDialog.grid_Master.DataStore)
        for eachProp in self.propDialog.grid_Display.DataStore:
            for i, eachMasterProp in enumerate(tempMasterData):
                if eachMasterProp[3] == eachProp[3]:
                    tempMasterData.pop(i)
        self.propDialog.grid_Master.DataStore = tempMasterData
        print "Property Tab"

    def OnTest(self, sender, e):
        try:
            #forms.GridView.
            #print forms.GridView.Columns.
            #forms.GridView.Visible
            #print self.grid.Columns[0].DataCell
            #print headersVisible
            pass
            #forms.GridView.DataStore.
            #forms.GridColumnCollection.
        except:
            print "OnTest Failed"

    #Change selection functions
    def AddByPicking(self, sender, e):
        print "AddByPicking() Running"
        try:
            rs.LockObjects(self.ObjsInRows())
            newObjs = rs.GetObjects("Select objects to add to selection", preselect = True)
            rs.UnlockObjects(self.ObjsInRows())
            if newObjs is None: return
            
            self.rows += newObjs
            
            #Save to sticky
            self.SetObjs2Sticky()
            
            #Regen
            self.Regen()
            print "AddByPicking() Succeeded"
        except:
            print "AddByPicking() Failed"

    def RemoveByPicking(self, sender, e):
        print "RemoveByPicking() Running"
        try:
            #Get objs to remove
            rs.EnableRedraw(False)
            tempLock = rs.InvertSelectedObjects(rs.SelectObjects(self.ObjsInRows()))
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
            print "RemoveByPicking() Succeeded"
        except:
            print "RemoveByPicking() Failed"

    def RemoveSelection(self, sender, e):
        try:
            #print "RemoveSelection() Running"
            #Get objs to remove
            items = list(self.grid.SelectedItems)
            if items is None: return
            
            itemNums = list(self.grid.SelectedRows)
            itemNums.sort(reverse = True)
            
            for itemNum in itemNums:
                self.RemoveByIndex(itemNum)
            
            self.Regen()
            print "RemoveSelection() Finished"
        except:
            print "RemoveSelection failed"

    def RemoveByIndex(self, index):
        try:
            #clean self.rows
            self.rows.pop(int(index))
        except:
            print "RemoveByIndex failed"

    def RemoveByGUID(self, guid):
        for i, obj in enumerate(self.rows):
            if str(obj) == str(guid):
                del self.rows[i]
                break
        
        print "INSIDE"
        
        #Remove from dataStore
        data = self.grid.DataStore
        for i, each in enumerate(data):
            if each[self.guid_Num] == guid:
                data.pop(i)
                print "One removed"
        self.grid.DataStore = data

    def ObjsInRows(self):
        objects = []
        for eachRow in self.rows:
            print eachRow
            if len(str(eachRow)) > 6:
                if rs.IsObject(str(eachRow)):
                    objects.append(eachRow)
        return objects
    
    #Regen
    def Regen(self, *args):
        try:
            #Remove blank GUIDs
            self.CleanGUIDs()
            #Regen Data
            self.RegenData2()
            #Regen Format
            self.RegenFormat()
        except:
            print "Regen failed"

    def CleanGUIDs(self):
        """
        Removes guid's not in the document from the Tabl_
        """
        try:
            count = 0
            print "self.rows:\n" + str(self.rows)
            for i, eachRow in enumerate(self.rows):
                if len(str(eachRow))>0:
                    if not rs.IsObject(str(eachRow)):
                        count += 1
                        self.rows.pop(i)
            if count > 0:
                print "{} objects from previous selection not found.".format(count)
            self.SetObjs2Sticky()
            print "CleanGUIDs() suceeded"
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
                    print "isCurve failed"
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
                    if self.colorFormat == 0:
                        color = str(rs.ObjectColor(obj).ToKnownColor())
                        if color == str(0):
                            color = "Other"
                    if self.colorFormat == 1:
                        color = str(rs.ObjectColor(obj).R) + "-" +str(rs.ObjectColor(obj).G) + "-" +str(rs.ObjectColor(obj).B)
                    if self.colorFormat == 2:
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
                    #print "type failed"

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
                    elif rs.IsPolysurface(obj):
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
            
            #Regened data
            objData = []
            for i, obj in enumerate(self.objs):
                try:
                    objData.append(RegenDataForObj(i, obj))
                except:
                    print "RegenDataForObj failed"
            
            #Existing data
            existingData = self.grid.DataStore
            
            if existingData is None:
                #For initial Regen
                self.grid.DataStore = objData
            else:
                for eachExisting in existingData:
                    for i, eachNew in enumerate(objData):
                        if eachExisting[self.guid_Num] == eachNew[self.guid_Num]:
                            eachExisting = eachNew
                            objData.pop(i)
                            break
                finalData = existingData + objData
                self.grid.DataStore = finalData
        except:
            print "RegenData() Failed"

    def RegenData2(self):
        try:
            #print "RegenData2 running\n____________"
            def GetProperty(rowNum, obj, propNum):
                
                def GetObjGUID(obj):
                    try:
                        return str(obj)
                    except:
                        return "GetObjGUID Fail"
                
                def GetObjType(obj):
                    try:
                        typeNum = rs.ObjectType(obj)
                        return self.typeDict[typeNum]
                    except:
                        return "GetObjType Fail"
                
                def GetObjName(obj):
                    try:
                        return rs.ObjectName(obj)
                    except:
                        return "GetObjName Fail"
                
                def GetObjLayer(obj):
                    try:
                        return rs.ObjectLayer(obj)
                    except:
                        return "GetObjLayer Fail"
                
                def GetObjDisplayColor(obj):
                    try:
                        return rs.ObjectColor(obj)
                    except:
                        return "GetObjColor Fail"
                
                def GetObjLinetype(obj):
                    try:
                        return str(rs.ObjectLinetype(obj))
                    except:
                        print "linetype failed"
                        return ""
                
                def GetObjPrintColor(obj):
                    try:
                        if self.colorFormat == 0:
                            p_color = str(rs.ObjectPrintColor(obj).ToKnownColor())
                            if p_color == str(0):
                                p_color = "Other"
                            return p_color
                        if self.colorFormat == 1:
                            return str(rs.ObjectPrintColor(obj).R) + "-" +str(rs.ObjectPrintColor(obj).G) + "-" +str(rs.ObjectPrintColor(obj).B)
                        if self.colorFormat == 2:
                            return str(rs.ObjectPrintColor(obj).R) + "," +str(rs.ObjectPrintColor(obj).G) + "," +str(rs.ObjectPrintColor(obj).B)
                    except:
                        print "p_color failed"
                        return ""
                
                def GetObjPrintWidth(obj):
                    try:
                        return str(rs.ObjectPrintWidth(obj))
                    except:
                        print "p_width failed"
                        return ""

                def GetObjRenderMaterial(obj):
                    try:
                        tempMat = rs.MaterialName(rs.ObjectMaterialIndex(obj))
                        if tempMat is None:
                            material = ""
                        else: material = str(tempMat)
                        return material
                    except:
                        return ""
                        print "material failed"

                def GetObjLength(obj):
                    try:
                        if IsCurve:
                            if rs.IsCurveClosed(obj):
                                area = str(rs.Area(obj))
                        if rs.IsBrep(obj):
                            area = str(rs.Area(obj))
                        try:
                            area
                        except:
                            area = ""
                        return area
                    except:
                        return ""
                        print "area failed"

                def GetObjArea(obj):
                    try:
                        if isCurve:
                            length = str(rs.CurveLength(obj))
                        if isSurface:
                            length = ""
                        try:
                            length
                        except:
                            length = ""
                        return length
                    except:
                        return None
                        print "length failed"

                def GetObjVolume(obj):
                    try:
                        if rs.IsPolysurface(obj):
                            if rs.IsPolysurfaceClosed(obj):
                                volume = str(rs.SurfaceVolume(obj)[0])
                        try:
                            volume
                        except:
                            volume = ""
                        return volume
                    except:
                        return None
                        print "volume failed"
                
                def IsObjPlanar(obj):
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
                        return isPlanar
                    except:
                        return ""
                        print "isPlanar failed"

                def IsObjClosed(obj):
                    try:
                        if isCurve:
                            isClosed = str(rs.IsCurveClosed(obj))
                        elif rs.IsPolysurface(obj):
                            isClosed = str(rs.IsPolysurfaceClosed(obj))
                        elif isSurface:
                            isClosed = str(rs.IsPolysurfaceClosed(obj))
                        else:
                            isClosed = ""
                        try:
                            isClosed
                        except:
                            isClosed = ""
                        return isClosed
                    except:
                        return ""
                        print "isClosed failed"
                
                
                #print ""
                #print "\tProcessing obj: " + str(obj)
                #print "\tProcessing obj with propNum :" + str(propNum)
                
                if propNum == 0:
                    result = rowNum+1
                    
                try:
                    if len(str(obj))>6:
                        pass
                    else:
                        return ""
                except:
                    return ""
                
                #Clear up some stuff
                try:
                    isCurve = rs.IsCurve(obj)
                    isSurface = rs.IsSurface(obj)
                except:
                    isCurve = False
                    isSurface = False
                    print "isCurve failed"
                
                
                if propNum == 1:
                    result = GetObjGUID(obj)
                if propNum == 2:
                    result = GetObjType(obj)
                if propNum == 3:
                    result = GetObjName(obj)
                if propNum == 4:
                    result = GetObjLayer(obj)
                if propNum == 5:
                    result = GetObjDisplayColor(obj)
                if propNum == 6:
                    result = GetObjDisplayColor(obj)
                if propNum == 7:
                    result = GetObjPrintColor(obj)
                if propNum == 8:
                    result = GetObjPrintWidth(obj)
                if propNum == 9:
                    result = GetObjRenderMaterial(obj)
                if propNum == 10:
                    result = GetObjLength(obj)
                if propNum == 11:
                    result = GetObjArea(obj)
                if propNum == 12:
                    result = GetObjVolume(obj)
                if propNum == 16:
                    result = IsObjPlanar(obj)
                if propNum == 17:
                    result = IsObjClosed(obj)
                #print "\tResult: " + str(result)
                return str(result)
            
            #RegenData2
            #-GetProperty
            #---GetObjName
            #---GetObjLayer
            #---GetObjColor
            
            #print "rows: " + str(self.rows)
            
            dataStore = []
            for i, eachRow in enumerate(self.rows):
                tempRow = []
                for eachProperty in self.activePropertiesFunc:
                    tempRow.append(GetProperty(i, eachRow, int(eachProperty)))
                dataStore.append(tempRow)
            #print "Received properties: " + str(dataStore)
            self.grid.DataStore = dataStore
            
            #print "RegenData2 finished\n___________"
        except:
            print "RegenData2 failed\n___________"

    def RegenFormat(self):
        try:
            #Re-sort the data
            self.sortData()
            #Totals
            self.showTotalsFunc()
            #Thousands Separator
            self.formatNumbers()
            #Format color
            self.formatColors()
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
                if len(item)>0:
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
        print "OnNameChanged() running"
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
                print "Regening"
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
            print "Manual Mode"
        else:
            sc.sticky['radioMode'] = self.radioMode.SelectedIndex
            print "Automatic Mode"
            self.Regen()

    def OnDeleteRhinoObject(self, sender, e):
        try:
            print  "OnDeleteRhinoObject() event"
            self.RemoveByGUID(e.ObjectId)
        except:
            print "OnDeleteRhinoObject Failed"

    def OnSizeChanged(self, *args):
        try:
            #self.propDialog.grid_Master.Height = self.Height-135
            #self.propDialog.grid_Display.Height = self.Height-135
            #print self.propDialog.grid_Display.Height
            self.grid.Height = self.Height-135
            print self.propDialog.grid_Display.Height
            sc.sticky['dialogSize'] = self.Size
        except:
            print "OnSizeChanged failed"

    #Get Active
    def activeHeadingsList(self):
        #old
        #currentHeadings = []
        #for i, each in enumerate(self.colNames):
        #    if self.checkboxes[i].Checked:
        #        currentHeadings.append(str(each))
        #return currentHeadings
        
        #new
        numColumns = self.grid.Columns.Count
        activeHeaders = []
        for each in range(numColumns):
            name = self.grid.Columns[each].HeaderText
            name = name[:-2]
            activeHeaders.append(name)
        return activeHeaders

    def activeDataList(self):
        #old
        #currentDataIndex = []

        #for i, each in enumerate(self.checkboxes):
        #    if each.Checked:
        #        currentDataIndex.append(i)

        #newDataStore = []
        #for row in self.grid.DataStore:
        #    rowAttribute = []
        #    for index in currentDataIndex:
        #        rowAttribute.append(row[index])
        #    newDataStore.append(rowAttribute)
        #return newDataStore
        
        #new
        activeData = []
        for row in self.grid.DataStore:
            objData = []
            for eachIndex in self.activeHeadingIndex:
                objData.append(row[int(eachIndex)])
            activeData.append(objData)
        return activeData

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
            print "sortingData"
            try:
                self.sortingBy
            except:
                print "no self.sortingBy!"
            print "self.sortingBy = {}".format(self.sortingBy)
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
        self.RemoveEvents()
        try:
            sc.sticky['dialogPos'] = self.Location
            self.Close()
        except:
            pass

    #Checkbox Events
    def onCheckboxChanged(self, sender, e):
        try:
            print "onCheckboxChanged is OBSOLETE"
            #checkboxName = sender.Text.split(" ")[0]
            #for i, item in enumerate(self.colNames):
            #    if checkboxName == item:
            #        number = i
            #        break

            #sc.sticky[self.paramStates[number]] = self.checkboxes[number].Checked
            #self.columns[number].Visible = self.checkboxes[number].Checked
        except:
            print "onCheckboxChanged() failed"

    def showUnitsChanged(self, sender, e):
        sc.sticky['showUnitsChecked'] = self.showUnits.Checked
        if self.showUnits.Checked:
            self.showUnitsFunc()
            print "Units Shown"
        else:
            self.hideUnitsFunc()
            print "Units Hidden"

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

    def showPreviewChanged(self, sender, e):
        sc.sticky['showPreviewChecked'] = self.showPreview.Checked
        if self.showPreview.Checked:
            self.CustomDisplayColor()
        else:
            print "Preview Hidden"

    #Functions
    def CustomDisplayColor(self):
        #color = System.Drawing.Color.Red
        print "CustomDisplayColor"
        #rs.GetColor()
        #test1 = forms.ColorDialog()
        #pt0 = Rhino.Geometry.Point3d(0,0,0)
        #pt1 = Rhino.Geometry.Point3d(0,100,100)
        #curve = Rhino.Geometry.Line(pt0, pt1)
        #Rhino.Display.DisplayPipeline.DrawCurve(curve, color, 2)
        #Rhino.Display.CustomDisplay.
        #reutrn None

    def formatNumbers(self):
        try:
            numColumns = [self.length_Num, self.area_Num, self.volume_Num, self.centerZ_Num]
            data = self.grid.DataStore
            for list in data:
                for numColumn in numColumns:
                    if list[numColumn] is not None:
                        if len(list[numColumn])>0:
                            if commaSep == 0:
                                list[numColumn] = "{0:.{prec}f}".format(float(list[numColumn]), prec = int(decPlaces))
                            if commaSep == 1:
                                list[numColumn] = "{0:,.{prec}f}".format(float(list[numColumn]), prec = int(decPlaces))
                            if commaSep == 2:
                                list[numColumn] = "{0:,.{prec}f}".format(float(list[numColumn]), prec = int(decPlaces)).replace(',', '.')
                            if commaSep == 3:
                                list[numColumn] = "{0:,.{prec}f}".format(float(list[numColumn]), prec = int(decPlaces)).replace(',', ' ')
            self.grid.DataStore = data
        except:
            print "Showing Thousands Separator Failed"

    def hideThousandsComma(self):
        try:
            print "Hiding thousands separator"
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
        try:
            #print "Rows: " + str(len(self.rows))
            #print "ObjsInRows: " + str(len(self.ObjsInRows()))
            self.countLbl.Text = "Selected: {} objects".format(len(self.ObjsInRows()))
        except:
            print "recountLabel failed"

    def formatColors(self):
        try:
            print "Formatting colors"
        except:
            print "formatColors failed"

    #I/O Functions
    def copyToClipboard(self, sender, e):
        try:
            string = self.DataStoreToHTML()
            rs.ClipboardText(string)
            print "Copied to clipboard"
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
        fileName = rs.SaveFileName("Save table", filter = "CSV Files (*.csv)|*.csv|HTML Files (*.html)|*.html|TXT Files (*.txt)|*.txt||")
        if fileName is None:
            return
        extension = fileName.split(".")[-1]
        try:
            f = open(fileName,'w')
        except IOError:
            print "Cannot save file. File already open."
            return
        try:
            if extension == "html":
                string = self.DataStoreToHTML()
            if extension == "csv":
                global commaSep
                
                prevCommaSep = commaSep
                prevColorFormat = self.colorFormat
                
                if self.colorFormat == 2:
                    self.colorFormat = 1
                
                if commaSep:
                    commaSep = False
                
                self.Regen()
                
                string = self.DataStoreToCSV()
                
                self.colorFormat = prevColorFormat
                commaSep = prevCommaSep
                
                self.Regen()
            if extension == "txt":
                string = self.DataStoreToTXT()
            f.write(string)
            f.close()
            print "Exported to {}".format(extension)
        except:
            print "exportData() Failed"

    def DataStoreToTXT(self):
        string = ""
        seperator = "\t"
        if self.showHeaders.Checked:
            allHeadings = self.activeHeadingsList()
            print "Headings received"
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
    #if sc.sticky.has_key('sample_modeless_form'):
    #    return
    print "____________________________"
    if 'decPlaces' in sc.sticky:
        global decPlaces
        decPlaces = sc.sticky['decPlaces']
        print "found decPlaces"
    else:
        global decPlaces
        decPlaces = 2
        
    if 'commaSep' in sc.sticky:
        global commaSep
        commaSep = sc.sticky['commaSep']
        print "found commaSep"
    else:
        global commaSep
        commaSep = False
        
    global version
    version = "Version 0.2.0"


    #commaSep = False
    #colorFormat = 1

    report = Tabl_()
    report.Owner = Rhino.UI.RhinoEtoApp.MainWindow
    report.Show()

    #sc.sticky['sample_modeless_form'] = report
    #rc = report.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow)

if __name__ == "__main__":
    main()