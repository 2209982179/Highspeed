import vue from 'vue'
import messageBox from './messageBox.vue'

// 创建vue组件实例
const MessageBox = vue.extend(messageBox)

const param = {
  dialogVisible: true,
  message: '这是一条提示消息',
  type: 'error|success|info|error|',
  buttons: [
    {
      text: '确定',
      type: 'primary|success|info|warning|danger',
      onClick: () => {},
    },
  ],
}

let myMessageBox = {
  /**
   *
   * @param {param} param0
   */
  alert: ({ dialogVisible, message, buttons, type, title, close }) => {
    // 创建一个存放对话框的div
    const MessageBoxDOM = new MessageBox({
      el: document.createElement('div'),
      data() {
        return {
          dialogVisible: dialogVisible,
          message: message,
          buttons: buttons,
          type: type,
          title,
          close,
        }
      },
    })
    document.body.appendChild(MessageBoxDOM.$el)
  },
}

// 注册
function register() {
  vue.prototype.$messageBox = myMessageBox // 暴露出去的方法名
  window.$messageBox = myMessageBox
}

export default { myMessageBox, register }
