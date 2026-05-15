const baseUrl = window.document.location.origin
window['mxBasePath'] = baseUrl + '/drawio/mxgraph'
window['mxImageBasePath'] = baseUrl + '/drawio/webapp/mxgraph/images'
window['mxDefaultLanguage'] = 'en'
window['mxLoadResources'] = false
window['mxLoadStylesheets'] = true
window['mxForceIncludes'] = false
window['mxResourceExtension'] = '.txt'
window.BASE_DRAW_APP_PATH = baseUrl + '/drawio/webapp/'

var urlParams = (function () {
  var result = new Object()
  var params = window.location.search.slice(1).split('&')

  for (var i = 0; i < params.length; i++) {
    var idx = params[i].indexOf('=')

    if (idx > 0) {
      result[params[i].substring(0, idx)] = params[i].substring(idx + 1)
    }
  }

  return result
})()

/**
 * Adds meta tag to the page.
 */
export function mxmeta(name, content, httpEquiv) {
  try {
    var s = document.createElement('meta')

    if (name != null) {
      s.setAttribute('name', name)
    }

    s.setAttribute('content', content)

    if (httpEquiv != null) {
      s.setAttribute('http-equiv', httpEquiv)
    }

    var t = document.getElementsByTagName('meta')[0]
    t.parentNode.insertBefore(s, t)
  } catch (e) {
    // ignore
  }
}

/**
 * Synchronously adds scripts to the page.
 * @param {[]} srcList
 * @param {function} onload
 * @param {int} cur
 */
export function loopInclude(srcList, onload, cur) {
  if (!cur) cur = 0
  if (cur + 1 === srcList.length) mxscript(srcList[cur], onload)
  else
    mxscript(srcList[cur], () => {
      loopInclude(srcList, onload, cur + 1)
    })
}

/**
 * Synchronously adds scripts to the page.
 */
export function mxscript(src, onLoad, id, dataAppKey, noWrite, onError) {
  if (
    typeof document.createElement('canvas').getContext === 'function' ||
    onLoad != null ||
    noWrite
  ) {
    var s = document.createElement('script')
    s.setAttribute('type', 'text/javascript')
    s.setAttribute('defer', 'true')
    s.setAttribute('src', src)

    if (id != null) {
      s.setAttribute('id', id)
    }

    if (dataAppKey != null) {
      s.setAttribute('data-app-key', dataAppKey)
    }

    if (onLoad != null) {
      var r = false

      s.onload = s.onreadystatechange = function () {
        if (!r && (!this.readyState || this.readyState == 'complete')) {
          r = true
          onLoad()
        }
      }
    }

    if (onError != null) {
      s.onerror = function (e) {
        onError('Failed to load ' + src, e)
      }
    }

    var t = document.getElementsByTagName('script')[0]

    if (t != null) {
      t.parentNode.insertBefore(s, t)
    }
  } else {
    document.write(
      '<script src="' +
        src +
        '"' +
        (id != null ? ' id="' + id + '" ' : '') +
        (dataAppKey != null ? ' data-app-key="' + dataAppKey + '" ' : '') +
        '></scr' +
        'ipt>'
    )
  }
}

window.urlParams = urlParams
window.mxscript = mxscript
window.loopInclude = loopInclude

var mxScriptsLoaded = false,
  mxWinLoaded = false

var t0 = new Date()

export function loadScripts(onload) {
  const mxgraphBasePath = baseUrl + '/drawio/mxgraph'
  const webappBasePath = baseUrl + '/drawio/webapp'
  const grapheditorBasePath = webappBasePath + '/js/grapheditor'
  const diagramlyBasePath = webappBasePath + '/js/diagramly'

  // function checkAllLoaded() {
  //   if (mxScriptsLoaded && mxWinLoaded) {
  //     App.main()
  //   }
  // }

  // if (typeof document.createElement('canvas').getContext === 'function') {
  //   window.addEventListener('load', function () {
  //     mxWinLoaded = true
  //     checkAllLoaded()
  //   })
  // } else {
  //   App.main()
  // }

  loopInclude(
    [
      webappBasePath + '/js/PreConfig.js',
      diagramlyBasePath + '/Init.js',
      grapheditorBasePath + '/Init.js',
      mxgraphBasePath + '/mxClient.js',
    ],
    () =>
      checkMxInclueds(loopInclude, () => {
        loopInclude(
          [
            webappBasePath + '/js/cryptojs/aes.min.js',
            webappBasePath + '/js/spin/spin.min.js',
            webappBasePath + '/js/deflate/pako.min.js',
            webappBasePath + '/js/deflate/base64.js',
            webappBasePath + '/js/jscolor/jscolor.js',
            webappBasePath + '/js/sanitizer/purify.min.js',
            webappBasePath + '/js/rough/rough.min.js',
            webappBasePath + '/js/freehand/perfect-freehand.js',
            grapheditorBasePath + '/Editor.js',
            grapheditorBasePath + '/EditorUi.js',
            grapheditorBasePath + '/Sidebar.js',
            grapheditorBasePath + '/Graph.js',
            grapheditorBasePath + '/Format.js',
            grapheditorBasePath + '/Shapes.js',
            grapheditorBasePath + '/Actions.js',
            grapheditorBasePath + '/Menus.js',
            grapheditorBasePath + '/Toolbar.js',
            grapheditorBasePath + '/Dialogs.js',
            diagramlyBasePath + '/sidebar/Sidebar.js',
            diagramlyBasePath + '/sidebar/Sidebar-ActiveDirectory.js',
            diagramlyBasePath + '/sidebar/Sidebar-Advanced.js',
            diagramlyBasePath + '/sidebar/Sidebar-AlliedTelesis.js',
            diagramlyBasePath + '/sidebar/Sidebar-Android.js',
            diagramlyBasePath + '/sidebar/Sidebar-ArchiMate.js',
            diagramlyBasePath + '/sidebar/Sidebar-ArchiMate3.js',
            diagramlyBasePath + '/sidebar/Sidebar-Arrows2.js',
            diagramlyBasePath + '/sidebar/Sidebar-Atlassian.js',
            diagramlyBasePath + '/sidebar/Sidebar-AWS.js',
            diagramlyBasePath + '/sidebar/Sidebar-AWS3.js',
            diagramlyBasePath + '/sidebar/Sidebar-AWS3D.js',
            diagramlyBasePath + '/sidebar/Sidebar-AWS4.js',
            diagramlyBasePath + '/sidebar/Sidebar-AWS4b.js',
            diagramlyBasePath + '/sidebar/Sidebar-Azure.js',
            diagramlyBasePath + '/sidebar/Sidebar-Azure2.js',
            diagramlyBasePath + '/sidebar/Sidebar-Basic.js',
            diagramlyBasePath + '/sidebar/Sidebar-Bootstrap.js',
            diagramlyBasePath + '/sidebar/Sidebar-BPMN.js',
            diagramlyBasePath + '/sidebar/Sidebar-C4.js',
            diagramlyBasePath + '/sidebar/Sidebar-Cabinet.js',
            diagramlyBasePath + '/sidebar/Sidebar-Cisco.js',
            diagramlyBasePath + '/sidebar/Sidebar-Cisco19.js',
            diagramlyBasePath + '/sidebar/Sidebar-CiscoSafe.js',
            diagramlyBasePath + '/sidebar/Sidebar-Citrix.js',
            diagramlyBasePath + '/sidebar/Sidebar-Cumulus.js',
            diagramlyBasePath + '/sidebar/Sidebar-DFD.js',
            diagramlyBasePath + '/sidebar/Sidebar-EIP.js',
            diagramlyBasePath + '/sidebar/Sidebar-Electrical.js',
            diagramlyBasePath + '/sidebar/Sidebar-ER.js',
            diagramlyBasePath + '/sidebar/Sidebar-Floorplan.js',
            diagramlyBasePath + '/sidebar/Sidebar-Flowchart.js',
            diagramlyBasePath + '/sidebar/Sidebar-FluidPower.js',
            diagramlyBasePath + '/sidebar/Sidebar-GCP.js',
            diagramlyBasePath + '/sidebar/Sidebar-GCP2.js',
            diagramlyBasePath + '/sidebar/Sidebar-GCPIcons.js',
            diagramlyBasePath + '/sidebar/Sidebar-Gmdl.js',
            diagramlyBasePath + '/sidebar/Sidebar-IBM.js',
            diagramlyBasePath + '/sidebar/Sidebar-Infographic.js',
            diagramlyBasePath + '/sidebar/Sidebar-Ios.js',
            diagramlyBasePath + '/sidebar/Sidebar-Ios7.js',
            diagramlyBasePath + '/sidebar/Sidebar-Kubernetes.js',
            diagramlyBasePath + '/sidebar/Sidebar-LeanMapping.js',
            diagramlyBasePath + '/sidebar/Sidebar-Mockup.js',
            diagramlyBasePath + '/sidebar/Sidebar-MSCAE.js',
            diagramlyBasePath + '/sidebar/Sidebar-Network.js',
            diagramlyBasePath + '/sidebar/Sidebar-Office.js',
            diagramlyBasePath + '/sidebar/Sidebar-PID.js',
            diagramlyBasePath + '/sidebar/Sidebar-Rack.js',
            diagramlyBasePath + '/sidebar/Sidebar-Salesforce.js',
            diagramlyBasePath + '/sidebar/Sidebar-Signs.js',
            diagramlyBasePath + '/sidebar/Sidebar-Sitemap.js',
            diagramlyBasePath + '/sidebar/Sidebar-Sysml.js',
            diagramlyBasePath + '/sidebar/Sidebar-ThreatModeling.js',
            diagramlyBasePath + '/sidebar/Sidebar-UML25.js',
            diagramlyBasePath + '/sidebar/Sidebar-Veeam.js',
            diagramlyBasePath + '/sidebar/Sidebar-Veeam2.js',
            diagramlyBasePath + '/sidebar/Sidebar-VVD.js',
            diagramlyBasePath + '/sidebar/Sidebar-WebIcons.js',
            diagramlyBasePath + '/util/mxJsCanvas.js',
            diagramlyBasePath + '/util/mxAsyncCanvas.js',
            diagramlyBasePath + '/DrawioFile.js',
            diagramlyBasePath + '/LocalFile.js',
            diagramlyBasePath + '/LocalLibrary.js',
            diagramlyBasePath + '/StorageFile.js',
            diagramlyBasePath + '/StorageLibrary.js',
            diagramlyBasePath + '/RemoteFile.js',
            diagramlyBasePath + '/RemoteLibrary.js',
            diagramlyBasePath + '/EmbedFile.js',
            diagramlyBasePath + '/Dialogs.js',
            diagramlyBasePath + '/Editor.js',
            diagramlyBasePath + '/EditorUi.js',
            diagramlyBasePath + '/DiffSync.js',
            diagramlyBasePath + '/Settings.js',
            diagramlyBasePath + '/DrawioFilePuller.js',
            diagramlyBasePath + '/DrawioFileSync.js',
            diagramlyBasePath + '/DrawioComment.js',
            diagramlyBasePath + '/DriveComment.js',
            diagramlyBasePath + '/DrawioClient.js',
            diagramlyBasePath + '/DrawioUser.js',
            diagramlyBasePath + '/UrlLibrary.js',
            diagramlyBasePath + '/DriveFile.js',
            diagramlyBasePath + '/DriveLibrary.js',
            diagramlyBasePath + '/DriveClient.js',
            diagramlyBasePath + '/DropboxFile.js',
            diagramlyBasePath + '/DropboxLibrary.js',
            diagramlyBasePath + '/DropboxClient.js',
            diagramlyBasePath + '/GitHubFile.js',
            diagramlyBasePath + '/GitHubLibrary.js',
            diagramlyBasePath + '/GitHubClient.js',
            diagramlyBasePath + '/OneDriveFile.js',
            diagramlyBasePath + '/OneDriveLibrary.js',
            diagramlyBasePath + '/OneDriveClient.js',
            webappBasePath + '/js/onedrive/mxODPicker.js',
            diagramlyBasePath + '/TrelloFile.js',
            diagramlyBasePath + '/TrelloLibrary.js',
            diagramlyBasePath + '/TrelloClient.js',
            diagramlyBasePath + '/GitLabFile.js',
            diagramlyBasePath + '/GitLabLibrary.js',
            diagramlyBasePath + '/GitLabClient.js',
            diagramlyBasePath + '/App.js',
            diagramlyBasePath + '/Menus.js',
            diagramlyBasePath + '/Pages.js',
            diagramlyBasePath + '/Trees.js',
            diagramlyBasePath + '/Minimal.js',
            diagramlyBasePath + '/DistanceGuides.js',
            diagramlyBasePath + '/mxRuler.js',
            diagramlyBasePath + '/mxFreehand.js',
            diagramlyBasePath + '/P2PCollab.js',
            diagramlyBasePath + '/DevTools.js',
            diagramlyBasePath + '/vsdx/VsdxExport.js',
            diagramlyBasePath + '/vsdx/mxVsdxCanvas2D.js',
            diagramlyBasePath + '/vsdx/bmpDecoder.js',
            diagramlyBasePath + '/vsdx/importer.js',
            webappBasePath + '/js/jszip/jszip.min.js',
            diagramlyBasePath + '/graphml/mxGraphMlCodec.js',
            diagramlyBasePath + '/miro/MiroImporter.js',
            webappBasePath + '/js/mermaid/mermaid.min.js',
            //webappBasePath + '/js/mermaid/mermaid2drawio.js',
            webappBasePath + '/js/PostConfig.js',
          ],
          () => {
            mxScriptsLoaded = true
            App.handleError()
            Editor.initMath()
            App.loadScripts(
              [
                'js/shapes-14-6-5.min.js',
                'js/stencils.min.js',
                'js/extensions.min.js',
              ],
              () => {
                if (onload) onload()
              }
            )
          }
        )
      })
  )
}

export function createGraph(container) {
  var app = new App(
    new Editor(
      urlParams['chrome'] == '0' || uiTheme == 'min',
      null,
      null,
      null,
      urlParams['chrome'] != '0'
    ),
    container
  )
  return app
}

import factory from 'mxgraph'
const { ActiveXObject } = factory({})

export const mxPrototype = {
  mxClient: window.mxClient,
  mxLog: window.mxLog,
  mxObjectIdentity: window.mxObjectIdentity,
  mxDictionary: window.mxDictionary,
  mxResources: window.mxResources,
  mxPoint: window.mxPoint,
  mxRectangle: window.mxRectangle,
  mxEffects: window.mxEffects,
  mxUtils: window.mxUtils,
  mxConstants: window.mxConstants,
  mxEventObject: window.mxEventObject,
  mxMouseEvent: window.mxMouseEvent,
  mxEventSource: window.mxEventSource,
  mxEvent: window.mxEvent,
  mxXmlRequest: window.mxXmlRequest,
  mxClipboard: window.mxClipboard,
  mxWindow: window.mxWindow,
  mxForm: window.mxForm,
  mxImage: window.mxImage,
  mxDivResizer: window.mxDivResizer,
  mxDragSource: window.mxDragSource,
  mxToolbar: window.mxToolbar,
  mxUndoableEdit: window.mxUndoableEdit,
  mxUndoManager: window.mxUndoManager,
  mxUrlConverter: window.mxUrlConverter,
  mxPanningManager: window.mxPanningManager,
  mxPopupMenu: window.mxPopupMenu,
  mxAutoSaveManager: window.mxAutoSaveManager,
  mxAnimation: window.mxAnimation,
  mxMorphing: window.mxMorphing,
  mxImageBundle: window.mxImageBundle,
  mxImageExport: window.mxImageExport,
  mxAbstractCanvas2D: window.mxAbstractCanvas2D,
  mxXmlCanvas2D: window.mxXmlCanvas2D,
  mxSvgCanvas2D: window.mxSvgCanvas2D,
  mxVmlCanvas2D: window.mxVmlCanvas2D,
  mxGuide: window.mxGuide,
  mxShape: window.mxShape,
  mxStencil: window.mxStencil,
  mxStencilRegistry: window.mxStencilRegistry,
  mxMarker: window.mxMarker,
  mxActor: window.mxActor,
  mxCloud: window.mxCloud,
  mxRectangleShape: window.mxRectangleShape,
  mxEllipse: window.mxEllipse,
  mxDoubleEllipse: window.mxDoubleEllipse,
  mxRhombus: window.mxRhombus,
  mxPolyline: window.mxPolyline,
  mxArrow: window.mxArrow,
  mxArrowConnector: window.mxArrowConnector,
  mxText: window.mxText,
  mxTriangle: window.mxTriangle,
  mxHexagon: window.mxHexagon,
  mxLine: window.mxLine,
  mxImageShape: window.mxImageShape,
  mxLabel: window.mxLabel,
  mxCylinder: window.mxCylinder,
  mxConnector: window.mxConnector,
  mxSwimlane: window.mxSwimlane,
  mxGraphLayout: window.mxGraphLayout,
  mxStackLayout: window.mxStackLayout,
  mxPartitionLayout: window.mxPartitionLayout,
  mxCompactTreeLayout: window.mxCompactTreeLayout,
  mxRadialTreeLayout: window.mxRadialTreeLayout,
  mxFastOrganicLayout: window.mxFastOrganicLayout,
  mxCircleLayout: window.mxCircleLayout,
  mxParallelEdgeLayout: window.mxParallelEdgeLayout,
  mxCompositeLayout: window.mxCompositeLayout,
  mxEdgeLabelLayout: window.mxEdgeLabelLayout,
  mxGraphAbstractHierarchyCell: window.mxGraphAbstractHierarchyCell,
  mxGraphHierarchyNode: window.mxGraphHierarchyNode,
  mxGraphHierarchyEdge: window.mxGraphHierarchyEdge,
  mxGraphHierarchyModel: window.mxGraphHierarchyModel,
  mxSwimlaneModel: window.mxSwimlaneModel,
  mxHierarchicalLayoutStage: window.mxHierarchicalLayoutStage,
  mxMedianHybridCrossingReduction: window.mxMedianHybridCrossingReduction,
  mxMinimumCycleRemover: window.mxMinimumCycleRemover,
  mxCoordinateAssignment: window.mxCoordinateAssignment,
  mxSwimlaneOrdering: window.mxSwimlaneOrdering,
  mxHierarchicalLayout: window.mxHierarchicalLayout,
  mxSwimlaneLayout: window.mxSwimlaneLayout,
  mxGraphModel: window.mxGraphModel,
  mxCell: window.mxCell,
  mxGeometry: window.mxGeometry,
  mxCellPath: window.mxCellPath,
  mxPerimeter: window.mxPerimeter,
  mxPrintPreview: window.mxPrintPreview,
  mxStylesheet: window.mxStylesheet,
  mxCellState: window.mxCellState,
  mxGraphSelectionModel: window.mxGraphSelectionModel,
  mxCellEditor: window.mxCellEditor,
  mxCellRenderer: window.mxCellRenderer,
  mxEdgeStyle: window.mxEdgeStyle,
  mxStyleRegistry: window.mxStyleRegistry,
  mxGraphView: window.mxGraphView,
  mxGraph: window.mxGraph,
  mxCellOverlay: window.mxCellOverlay,
  mxOutline: window.mxOutline,
  mxMultiplicity: window.mxMultiplicity,
  mxLayoutManager: window.mxLayoutManager,
  mxSwimlaneManager: window.mxSwimlaneManager,
  mxTemporaryCellStates: window.mxTemporaryCellStates,
  mxCellStatePreview: window.mxCellStatePreview,
  mxConnectionConstraint: window.mxConnectionConstraint,
  mxGraphHandler: window.mxGraphHandler,
  mxPanningHandler: window.mxPanningHandler,
  mxPopupMenuHandler: window.mxPopupMenuHandler,
  mxCellMarker: window.mxCellMarker,
  mxSelectionCellsHandler: window.mxSelectionCellsHandler,
  mxConnectionHandler: window.mxConnectionHandler,
  mxConstraintHandler: window.mxConstraintHandler,
  mxRubberband: window.mxRubberband,
  mxHandle: window.mxHandle,
  mxVertexHandler: window.mxVertexHandler,
  mxEdgeHandler: window.mxEdgeHandler,
  mxElbowEdgeHandler: window.mxElbowEdgeHandler,
  mxEdgeSegmentHandler: window.mxEdgeSegmentHandler,
  mxKeyHandler: window.mxKeyHandler,
  mxTooltipHandler: window.mxTooltipHandler,
  mxCellTracker: window.mxCellTracker,
  mxCellHighlight: window.mxCellHighlight,
  mxDefaultKeyHandler: window.mxDefaultKeyHandler,
  mxDefaultPopupMenu: window.mxDefaultPopupMenu,
  mxDefaultToolbar: window.mxDefaultToolbar,
  mxEditor: window.mxEditor,
  mxCodecRegistry: window.mxCodecRegistry,
  mxCodec: window.mxCodec,
  mxObjectCodec: window.mxObjectCodec,
  mxCellCodec: window.mxCellCodec,
  mxModelCodec: window.mxModelCodec,
  mxRootChangeCodec: window.mxRootChangeCodec,
  mxChildChangeCodec: window.mxChildChangeCodec,
  mxTerminalChangeCodec: window.mxTerminalChangeCodec,
  mxGenericChangeCodec: window.mxGenericChangeCodec,
  mxGraphCodec: window.mxGraphCodec,
  mxGraphViewCodec: window.mxGraphViewCodec,
  mxStylesheetCodec: window.mxStylesheetCodec,
  mxDefaultKeyHandlerCodec: window.mxDefaultKeyHandlerCodec,
  mxDefaultToolbarCodec: window.mxDefaultToolbarCodec,
  mxDefaultPopupMenuCodec: window.mxDefaultPopupMenuCodec,
  mxEditorCodec: window.mxEditorCodec,
  ActiveXObject,
}