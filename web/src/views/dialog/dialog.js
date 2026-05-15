import Vue from 'vue'

let dic = {}

/**
 * 构建dialog，挂在#app
 * @param {*} 视图
 * @param {*} 数据
 */
function dialog(view, data, canCheckDiagramIsSaved = false) {
  let action = async () => {
    // 移除已有弹窗，确保只有一个弹窗显示
    if (dic[view.name]) {
      dic[view.name].dialogPopVisible = false
      document.body.querySelector('#app').removeChild(dic[view.name].$el)
      dic[view.name] = null
    }
    // 当dialog显隐时，变更dialog的数量。当dialog数量大于0时，graph不触发键盘事件
    if (!view['watch']) {
      view['watch'] = {}
    }
    if (!view.watch['dialogPopVisible']) {
      view.watch['dialogPopVisible'] = function (val) {
        let num = Vue.prototype.$store.getters['dialogs']
        if (val) {
          num++
        } else {
          num--
          dic[view.name] = null
          if (data && data.onDialogClose) {
            data.onDialogClose()
          }
        }
        Vue.prototype.$store.commit('dialogs', num)
      }
    }
    let PopupBox = Vue.extend(view)
    dic[view.name] = new PopupBox({
      data,
    }).$mount()
    // 挂载至页面
    document.body.querySelector('#app').appendChild(dic[view.name].$el)
    Vue.nextTick(() => {
      let _vm = dic[view.name]
      if (_vm) {
        _vm.dialogPopVisible = true
      }
    })
  }
  canCheckDiagramIsSaved = false // 设为不弹提示
  if (canCheckDiagramIsSaved) {
    // 先保存图，然后viewInDiagram
    let diagram = Vue.prototype.$store.getters['currentDiagram']
    window.$DiagramUtils.CheckDiagramIsSaved(diagram, action)
  } else {
    action()
  }
}

Vue.prototype.$showDialog = dialog
window.$showDialog = dialog
