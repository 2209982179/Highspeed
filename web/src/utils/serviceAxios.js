import axios from 'axios'
import qs from 'qs'
import fileDownload from 'js-file-download'
import { UngzipToString, GZipToByteArray, CheckSum } from '../utils/gzip.js'

if (window.appsettings && window.appsettings.baseURL) {
  axios.defaults.baseURL = window.appsettings.baseURL
} else {
  axios.defaults.baseURL = process.env.VUE_APP_BASE_API
}
console.log('baseURL', axios.defaults.baseURL)
axios.defaults.withCredentials = true
axios.defaults.crossDomain = true
axios.defaults.headers.common['Access-Control-Allow-Origin'] = '*'

// 创建 axios 请求实例
const serviceAxios = axios.create()

// 创建请求拦截
serviceAxios.interceptors.request.use(
  async (config) => {
    // 如果开启 token 认证
    // 当前窗口的项目ID
    if (window.$project) config.headers['projectId'] = window.$project.ProjectId

    // 加入当前选中的内容
    var arr = [],
      packageIds = [],
      diagramIds = [],
      elementIds = [],
      attributeIds = [],
      connectorIds = []
    arr.forEach((r) => {
      // 组合模型存在无RealID的情况，需要过滤
      if (r.RealID) {
        switch (r.type) {
          case 'Package':
            packageIds.push(r.RealID)
            break
          case 'Diagram':
            diagramIds.push(r.RealID)
            break
          case 'Element':
            elementIds.push(r.RealID)
            break
          case 'Attribute':
            attributeIds.push(r.RealID)
            break
          case 'Connector':
            connectorIds.push(r.RealID)
            break
        }
      }
    })
    config.headers['SelectedIds'] = JSON.stringify({
      PackageIds: [...new Set(packageIds)],
      DiagramIds: [...new Set(diagramIds)],
      ElementIds: [...new Set(elementIds)],
      AttributeIds: [...new Set(attributeIds)],
      ConnectorIds: [...new Set(connectorIds)],
    })

    // 设置请求头
    if (!config.headers['Content-Type']) {
      // 如果没有设置请求头
      switch (config.method) {
        case 'post':
        case 'form':
          config.method = 'post'
          config.headers['Content-Type'] = 'application/x-www-form-urlencoded' // post 请求
          if (config.data) config.data = qs.stringify(config.data) // 序列化,比如表单数据
          break
        case 'json':
          config.method = 'post'
          config.headers['Content-Type'] = 'application/json' // 默认类型
          break
        case 'gzip-json':
          await GZipToByteArray(config.data).then(async (zipped) => {
            config.method = 'post'
            config.headers['Content-Type'] = 'application/gzip-json'
            config.data = zipped.buffer
          })
          break
        case 'stream':
          config.method = 'post'
          config.headers['Content-Type'] = 'application/octet-stream' // 流
          break
        default:
          break
      }
    }
    return config
  },
  (error) => {
    console.error(error)
    Promise.reject(error)
  }
)

// 创建响应拦截
serviceAxios.interceptors.response.use(
  async (res) => {
    // 流返回的处理
    if (
      res.data.constructor.name === 'Blob' ||
      res.data.constructor === ArrayBuffer
    ) {
      // 流返回的JSON
      if (res.headers['content-type'].indexOf('application/json') > -1) {
        const blob =
          res.data.constructor === ArrayBuffer
            ? new Blob([res.data], {
                type: res.headers['content-type'],
              })
            : res.data
        const jsonData = await new Promise((resolve, reject) => {
          const reader = new FileReader()
          reader.onload = () => resolve(JSON.parse(reader.result))
          reader.onerror = (error) => reject(error)
          reader.readAsText(blob)
        })
        let body = jsonData

        // 返回Data结构
        if (body.Success) {
          return body.Data
        } else {
          console.error(res.config.url, body)
          return Promise.reject(body.Message || '处理失败')
        }
      }
      // gzip压缩的JSON
      else if (res.headers['content-type'] === 'application/gzip-json') {
        let str = null
        await UngzipToString(res.data)
          .then((val) => (str = val))
          .catch((err) => console.log('ungzip error: ' + err))
        let result = JSON.parse(str)
        result['checkSum'] = await CheckSum(res.data)
        return result
      }
      // 文件流直接下载
      else
        try {
          const blob = new Blob([res.data], {
            type: res.headers['content-type'],
          })
          const fileName = res.headers['file-name']
          fileDownload(blob, decodeURIComponent(fileName))
          return null
        } catch (ex) {
          return Promise.reject(ex)
        }
    } else {
      // 返回Data结构
      let body = res.data
      if (body.Success) {
        return body.Data
      } else {
        console.error(res.config.url, body)
        return Promise.reject(body.Message || '处理失败')
      }
    }
  },
  async (error) => {
    if (error && error.response) {
      let message
      switch (error.response.status) {
        case 302:
          message = '接口重定向了！'
          break
        case 400:
          message = '参数不正确！'
          break
        case 401:
          message = '网页问题！'
          break
        case 403:
          message = '您没有权限操作！'
          break
        case 4031:
          message = '已在其他终端登录！'
          break
        case 4032:
          message = '单个终端只允许一个用户登录！'
          break
        case 4033:
          message = '超过允许连接终端数！'
          break
        case 4034:
          message = '不在允许的终端列表中！'
          break
        case 404:
          message = `请求地址出错: ${error.response.config.url}`
          break
        case 408:
          message = '请求超时！'
          break
        case 409:
          message = '系统已存在相同数据！'
          break
        case 500:
          message = '服务器内部错误！'
          break
        case 501:
          message = '服务未实现！'
          break
        case 502:
          message = '网关错误！'
          break
        case 503:
          message = '服务不可用！'
          break
        case 504:
          message = '服务暂时无法访问，请稍后再试！'
          break
        case 505:
          message = 'HTTP 版本不受支持！'
          break
        default:
          message = '请求异常！'
          break
      }
      console.error(message)
      return Promise.reject(message)
    } else {
      console.error(error)
      return Promise.reject(error)
    }
  }
)
export default serviceAxios
