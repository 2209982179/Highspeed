"use strict";

var _interopRequireDefault = require("C:/AvionicsDAPlatform/SourceCode/ADAP/adap_web/node_modules/@babel/runtime/helpers/interopRequireDefault.js").default;
Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.default = void 0;
var _react = _interopRequireDefault(require("react"));
var _propTypes = _interopRequireDefault(require("prop-types"));
var _antd = require("antd");
var _imageShape = _interopRequireDefault(require("./shape-config/image-shape"));
require("./sidebar.less");
const {
  Panel
} = _antd.Collapse;
const SIDEBAR_BASIC_SHAPES = [{
  name: 'rectangle',
  key: 'Rectangle',
  logo: 'https://img.alicdn.com/tfs/TB19O8OokvoK1RjSZFNXXcxMVXa-33-26.svg',
  width: 120,
  height: 60
}, {
  name: 'rounded rectangle',
  key: 'Rounded Rectangle',
  logo: 'https://img.alicdn.com/tfs/TB1rzVHojDpK1RjSZFrXXa78VXa-33-26.svg',
  width: 120,
  height: 60
}, {
  name: 'trapezoid',
  key: 'Trapezoid',
  logo: 'https://img.alicdn.com/tfs/TB1nEXPokvoK1RjSZPfXXXPKFXa-33-26.svg',
  width: 120,
  height: 60
}, {
  name: 'circle',
  key: 'Circle',
  logo: 'https://img.alicdn.com/tfs/TB15iXQogHqK1RjSZFkXXX.WFXa-38-38.svg',
  width: 80,
  height: 80
}, {
  name: 'triangle',
  key: 'Triangle',
  logo: 'https://img.alicdn.com/tfs/TB1cxNKohTpK1RjSZR0XXbEwXXa-38-38.svg',
  width: 80,
  height: 80
}, {
  name: 'line',
  key: 'Line',
  logo: 'https://img.alicdn.com/tfs/TB1LOxPoirpK1RjSZFhXXXSdXXa-38-38.svg',
  width: 80,
  height: 80
}, {
  name: 'text',
  key: 'Text',
  logo: '',
  width: 60,
  height: 20
}];
const SIDEBAR_SVG_SHAPES = [{
  name: 'on-grid energy',
  key: 'shangwangdianliang',
  logo: 'https://img.alicdn.com/tfs/TB1i4I1wxTpK1RjSZR0XXbEwXXa-80-80.svg',
  width: 80,
  height: 80
}, {
  name: 'electricity earnings',
  key: 'fadianshouyi',
  logo: 'https://img.alicdn.com/tfs/TB1MVA2wr2pK1RjSZFsXXaNlXXa-80-80.svg',
  width: 80,
  height: 80
}, {
  name: 'monitor',
  key: 'gaojingjianshi',
  logo: 'https://img.alicdn.com/tfs/TB1DildwNYaK1RjSZFnXXa80pXa-80-80.svg',
  width: 80,
  height: 80
}];
const SIDEBAR_CARD_SHAPES = [{
  name: 'primary equipment',
  key: 'zhushebei',
  logo: 'https://img.alicdn.com/tfs/TB1eD9LdgHqK1RjSZJnXXbNLpXa-144-128.png',
  width: 100,
  height: 80
}, {
  name: 'auxiliary equipment',
  key: 'fujiashebei',
  logo: 'https://img.alicdn.com/tfs/TB1ejUeiAPoK1RjSZKbXXX1IXXa-36-32.png',
  width: 100,
  height: 80
}, {
  name: 'product element',
  key: 'chanchuwu',
  logo: 'https://img.alicdn.com/tfs/TB1ht.aisbpK1RjSZFyXXX_qFXa-32-32.png',
  width: 100,
  height: 80
}];
class SideBar extends _react.default.Component {
  constructor(props) {
    super(props);
    this.state = {};
  }
  componentDidMount() {}
  componentDidUpdate() {
    this.handleSidebarItems();
  }
  handleSidebarItems() {
    const {
      editor
    } = this.props;
    if (editor && editor.initSidebar) {
      const sidebarItems = document.querySelectorAll('.custom-sidebar-node');
      const newSidebarItems = Array.from(sidebarItems).filter(item => {
        if (!item.classList.contains('has-inited')) {
          item.classList.add('has-inited');
          return true;
        }
        return false;
      });
      editor.initSidebar(newSidebarItems);
    }
  }
  onChange() {
    setTimeout(() => {
      this.handleSidebarItems();
    }, 1000);
  }
  render() {
    const h = arguments[0];
    return h("div", {
      "attrs": {
        "className": "J_Sidebar_Container sidebar-container"
      }
    }, [h(_antd.Collapse, {
      "attrs": {
        "bordered": false,
        "defaultActiveKey": ['common', 'svg', 'picture', 'card']
      },
      "on": {
        "change": () => {
          this.onChange();
        }
      }
    }, [h(Panel, {
      "key": "common",
      "attrs": {
        "header": "basic"
      }
    }, [SIDEBAR_BASIC_SHAPES.map(shape => h("a", {
      "attrs": {
        "href": "javascript:void(0);",
        "className": "geItem custom-sidebar-node common-panel-node",
        "data-shape-type": "general",
        "data-shape-name": shape.key,
        "data-shape-label": shape.name,
        "data-shape-width": shape.width,
        "data-shape-height": shape.height
      },
      "key": `panel_a_${shape.key}`
    }, [h(_antd.Tooltip, {
      "attrs": {
        "placement": "top",
        "title": shape.name,
        "className": "tooltip"
      },
      "key": `panel_${shape.key}`
    }, [shape.logo ? h("img", {
      "attrs": {
        "className": "sidebar-node-image",
        "src": shape.logo,
        "alt": ""
      }
    }) : shape.key, h("span", {
      "attrs": {
        "className": "sidebar-node-label"
      }
    }, [shape.name])])]))]), h(Panel, {
      "attrs": {
        "header": "svg shapes"
      },
      "key": "svg"
    }, [SIDEBAR_SVG_SHAPES.map(shape => h("a", {
      "attrs": {
        "href": "javascript:void(0);",
        "className": "geItem custom-sidebar-node common-panel-node",
        "data-shape-type": "svg",
        "data-shape-name": shape.key,
        "data-shape-label": shape.name,
        "data-shape-width": shape.width,
        "data-shape-height": shape.height
      },
      "key": `panel_a_${shape.key}`
    }, [h(_antd.Tooltip, {
      "attrs": {
        "placement": "top",
        "title": shape.name,
        "className": "tooltip"
      },
      "key": `panel_${shape.key}`
    }, [h("img", {
      "attrs": {
        "className": "sidebar-node-image",
        "src": shape.logo,
        "alt": ""
      }
    }), h("span", {
      "attrs": {
        "className": "sidebar-node-label"
      }
    }, [shape.name])])]))]), h(Panel, {
      "attrs": {
        "header": "images"
      },
      "key": "picture"
    }, [_imageShape.default.map(shape => h("a", {
      "on": {
        "click": e => {
          e.preventDefault();
          return false;
        }
      },
      "key": `panel_a_${shape.key}`,
      "attrs": {
        "href": "a",
        "className": "geItem custom-sidebar-node",
        "data-shape-type": "image",
        "data-shape-width": shape.width,
        "data-shape-height": shape.height,
        "data-shape-name": shape.key,
        "data-shape-label": shape.name,
        "title": shape.name
      }
    }, [h(_antd.Tooltip, {
      "attrs": {
        "placement": "top",
        "title": shape.name,
        "className": "tooltip"
      },
      "key": `panel_${shape.key}`
    }, [h("img", {
      "attrs": {
        "className": "sidebar-node-image",
        "src": shape.logo,
        "alt": ""
      }
    }), h("span", {
      "attrs": {
        "className": "sidebar-node-label"
      }
    }, [shape.name])])]))]), h(Panel, {
      "attrs": {
        "header": "cards"
      },
      "key": "card"
    }, [SIDEBAR_CARD_SHAPES.map(shape => h("a", {
      "on": {
        "click": e => {
          e.preventDefault();
          return false;
        }
      },
      "key": `panel_a_${shape.key}`,
      "attrs": {
        "href": "a",
        "className": "geItem custom-sidebar-node",
        "data-shape-type": "card",
        "data-shape-width": shape.width,
        "data-shape-height": shape.height,
        "data-shape-name": shape.key,
        "data-shape-label": shape.name,
        "title": shape.name
      }
    }, [h(_antd.Tooltip, {
      "attrs": {
        "placement": "top",
        "title": shape.name,
        "className": "tooltip"
      },
      "key": `panel_${shape.key}`
    }, [h("img", {
      "attrs": {
        "className": "sidebar-node-image",
        "src": shape.logo,
        "alt": ""
      }
    }), h("span", {
      "attrs": {
        "className": "sidebar-node-label"
      }
    }, [shape.name])])]))])])]);
  }
}
exports.default = SideBar;
SideBar.propTypes = {
  editor: _propTypes.default.object
};

// Specifies the default values for props:
SideBar.defaultProps = {
  editor: {}
};