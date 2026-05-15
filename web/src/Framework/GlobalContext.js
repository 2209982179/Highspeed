import { MessageBox } from 'element-ui'
import Vue from 'vue'
export default {
  // 全局组件
  toolbox: null,
  ribbonBar: null,
  projectTree: null,
  diagramManager: null,
  PropertyEditor: null,
  /**临时图下标，用于生成序号和唯一标识 */
  TempDiagramIndex: 0,
  /**统一对话框 */
  messageBox: {
    error(text, title, config) {
      Vue.prototype.$hideLoading()
      if (!text) {
        text = '操作失败'
      }
      if (!config) config = {}
      config['cancelButtonText'] = '关闭'
      config['type'] = 'error' // ‘success’(成功) /warning(警告)/info(消息)/error(错误)/;
      config['showConfirmButton'] = false //是否显示取消按钮
      config['showCancelButton'] = true //是否显示取消按钮
      config['showClose'] = false //是否显示右上角的x
      config['closeOnClickModal'] = false //是否可以点击空白处关闭弹窗
      MessageBox.confirm(text, title ?? '错误', config)
    },
  },
  /**隐藏右键菜单 */
  HideContextMenu() {
    window.$ContextMenu.Hide()
  },
}
