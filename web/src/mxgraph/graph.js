import factory from "mxgraph";

// window["mxBasePath"] = "./mxgraph/src";
// window["mxImageBasePath"] = "./mxgraph/src/images";
// window["mxDefaultLanguage"] = "en";
// window["mxLoadResources"] = false;
// window["mxLoadStylesheets"] = true;
// window["mxForceIncludes"] = false;
// window["mxResourceExtension"] = ".txt";

const mxgraph = factory({});

//fix BUG https://github.com/jgraph/mxgraph/issues/49
// window["mxGraph"] = mxgraph.mxGraph;
// window["mxConstants"] = mxgraph.mxConstants;
// window["mxGraphModel"] = mxgraph.mxGraphModel;
// window["mxEditor"] = mxgraph.mxEditor;
// window["mxGeometry"] = mxgraph.mxGeometry;
// window["mxDefaultKeyHandler"] = mxgraph.mxDefaultKeyHandler;
// window["mxDefaultPopupMenu"] = mxgraph.mxDefaultPopupMenu;
// window["mxStylesheet"] = mxgraph.mxStylesheet;
// window["mxDefaultToolbar"] = mxgraph.mxDefaultToolbar;
// window["mxPerimeter"] = mxgraph.mxPerimeter;
// window["mxEdgeStyle"] = mxgraph.mxEdgeStyle;

export default mxgraph;