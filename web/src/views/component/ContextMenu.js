// 基于mxPopupMenu实现的右键菜单
import Vue from 'vue'
import mxgraph from '@/mxgraph/graph'
const { mxPopupMenu } = mxgraph

/**
 * 显示右键菜单
 * @param {*} menuItem
 * @param {*} x
 * @param {*} y
 * @param {*} evt
 */
function Show(menuItem, x, y, cell, evt) {
  if (!menuItem) {
    return
  }
  let addMenuItem = (topmenu, items, parent, isTopMenu) => {
    const groupcollection = Object.groupBy(items, ({ group }) => group)
    Object.keys(groupcollection).forEach((groupkey) => {
      // 加入菜单项
      groupcollection[groupkey].forEach((mi) => {
        // 标题
        let title = ['', null, undefined].includes(mi?.menuTitle)
          ? mi.title
          : mi.menuTitle

        // 事件
        let funct =
          ![null, undefined].includes(mi.menuItem) && mi.menuItem.length > 0
            ? null
            : async () => {
                if (mi.action) {
                  await mi.action()
                }
              }

        // 禁用
        let enable = mi.enable ?? !mi.disabled
        let subMenu = mi.children ?? mi.menuItem
        // 如果当前有子菜单进行递归
        if (subMenu?.length > 0) {
          // 加入无事件父级菜单
          let pmenu = topmenu.addItem(
            title,
            mi.image,
            () => {},
            parent,
            mi.iconCls,
            enable,
            () => {},
            mi.noHover
          )
          if (mi.action) {
            // 因为调试时发现在分组情况下，父菜单的点击UI存在缺陷，所以在加入子菜单之前加入一个菜单
            topmenu.addItem(
              title,
              null,
              funct,
              pmenu,
              null,
              enable,
              mi.active,
              mi.noHover
            )
            topmenu.addSeparator(pmenu, true)
          }
          // 加入子菜单
          addMenuItem(topmenu, subMenu, pmenu)
        } else {
          // 忽略无事件的菜单
          if (mi.action) {
            topmenu.addItem(
              title,
              mi.image,
              funct,
              parent,
              mi.iconCls,
              enable,
              mi.active,
              mi.noHover
            )
          }
        }
      })
      // 仅顶层菜单，在每组之间加分割线
      if (isTopMenu == true) {
        topmenu.addSeparator(parent, true)
      }
    })
  }
  // 隐藏当前打开的右键菜单
  window.$GlobalContext.HideContextMenu()
  mxPopupMenu.prototype.hideMenu()
  // 初始化并生成菜单
  mxPopupMenu.prototype.init()
  mxPopupMenu.prototype.autoExpand = true
  mxPopupMenu.prototype.factoryMethod = (menu, cell, event) => {
    addMenuItem(menu, menuItem, null, true)
  }
  // 选项过多时，提供滚动条
  var mxPopupMenuShowMenu = mxPopupMenu.prototype.showMenu
  mxPopupMenu.prototype.showMenu = function () {
    this.div.style.overflowY = 'auto'
    this.div.style.overflowX = 'hidden'
    this.div.style.maxHeight = '450px'
    mxPopupMenuShowMenu.apply(this, arguments)
  }
  // 设置菜单显示的位置
  mxPopupMenu.prototype.popup(x, y, cell, evt)
}

function Hide() {
  mxPopupMenu.prototype.hideMenu()
}

Vue.prototype.$ContextMenu = {
  Show,
  Hide,
}
window.$ContextMenu = {
  Show,
  Hide,
}
