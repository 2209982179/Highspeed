<template>
  <div id="app">
    <router-view></router-view>
  </div>
</template>

<script>
  // require('mxgraph/javascript/src/css/common.css')
  export default {
    name: 'App',
    methods: {
      handleOpen(key, keyPath) {
        console.log(key, keyPath)
      },
      handleClose(key, keyPath) {
        console.log(key, keyPath)
      },
    },
    mounted() {
      document
        .querySelector('#app')
        .addEventListener('contextmenu', function (event) {
          if (
            event.target.className ==
              'loadindWrap animation-enter animation-enter-active' ||
            event.target.className == 'el-dialog__wrapper'
          ) {
            // dialog和loading的灰色区域阻止默认的右键菜单弹出
            event.preventDefault()
          }
        })
    },
  }
</script>

<style lang="scss">
  * {
    padding: 0;
    margin: 0;
    font-size: 12px;
    // color: #333;
    font-family: 'Tahoma';
  }

  .title {
    font-size: 15px;
    color: #333;
    font-weight: Semibold;
  }

  .el-collapse-item__header {
    padding-left: 30px;
    font-size: 15px;
    color: #333;
    font-weight: Semibold;
    text-wrap: nowrap;
    text-align: left;
    text-overflow: ellipsis;
  }

  .el-button.is-loading span {
    color: #fff;
  }
  .el-button.el-button--primary span {
    color: #fff;
  }
  .el-button.el-button--danger span {
    color: #fff;
  }

  button[disabled] {
    color: #999 !important;
  }
  button[disabled] i {
    color: #999 !important;
  }
  button[disabled] span {
    color: #999 !important;
  }
  button[disabled] span {
    color: #999 !important;
  }
  li.is-disabled {
    color: #999 !important;
  }
  li.is-disabled span {
    color: #999 !important;
  }

  @import './style/dialog.scss';
  @import './style/DialogPopup.scss';
  @import './style/context-menu.scss';
  @import './style/general-shap.css';
  @import './style/allocate.scss';
  @import './style/elemtui.scss';
  @import './style/table.scss';
  @import './style/tabs.scss';
  @import './style/deep-el-tree.css';
  @import './style/deep-el-tree.css';
  #app {
    height: 100%;
  }
  /* .header {
  height: 8vh !important;
} */
  .aside {
    height: 100vh;
    border: 1px solid #ededed;
  }

  .menu-row {
    width: 100%;
  }

  .main {
    height: 100vh;
    border: 1px solid #ededed;
    background-color: #efefef;
    padding: 0;
  }

  .graph_container {
    width: 100%;
    height: 100%;
    background-image: url('./assets/grid.gif');
    background-color: #fff;
  }

  .mxCellEditor.mxPlainTextEditor {
    position: absolute;
  }

  .MainPageMenuViewTitle {
    font-size: 14px;
    font-weight: bold;
    font-family: 'Tahoma';
    text-align: left;
    padding-top: 10px;
    padding-bottom: 8px;
    padding-left: 10px;
    border-bottom: 1px solid #e1e1e1;

    .titleIcon {
      margin-right: 2px;
    }

    .title {
      height: 18px;
      line-height: 18px;
      font-size: 15px;
      color: #333;
      font-weight: Semibold;
    }
    i {
      font-size: 15px;
    }

    .right-icon-btn {
      margin-right: 10px;
      float: right;
      cursor: pointer;
    }
  }

  .MainPageFilter {
    height: 32px;
  }

  .verticalView {
    background: #efefef;
    text-align: center;
    background-color: #fff;
    box-sizing: border-box;
    -webkit-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
    min-height: 100%;
    max-height: 100%;
    height: 100%;
  }
  .elemnt-icons {
    font-size: 22px;
  }
  /* 方块图标按钮 */
  .blockbtn {
    padding: 8px !important;
    margin-left: 10px;
  }
  .geIcon {
    font-size: 16px !important;
    margin-right: -20px !important;
  }
</style>
