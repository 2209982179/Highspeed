export const signalType = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择信号类型'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const signalName = [
  { required: true, message: '请输入信号名称', trigger: 'blur' },
  { max: 100, message: '长度在100个字符以内', trigger: 'blur' },
]

export const Name = [
  { required: true, message: '请输入名称', trigger: 'blur' },
  { max: 100, message: '长度在100个字符以内', trigger: 'blur' },
]

export const name = [
  { required: true, message: '请输入名称', trigger: 'blur' },
  { max: 100, message: '长度在100个字符以内', trigger: 'blur' },
]

export const newname = [
  { required: true, message: '请输入新名称', trigger: 'blur' },
  { max: 100, message: '长度在100个字符以内', trigger: 'blur' },
]

export const pbstype = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择系统类型'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const subsystem = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择子系统'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const classStereotype = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择元素类型'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const portSteretype = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择端口类型'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const Steretype = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择元素类型'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const signal = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择信号'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const senderPort = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择发送方端口'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const receiverPort = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择接收方端口'))
      } else {
        callback()
      }
    },
    trigger: 'blur',
  },
]

export const clsName = [
  { required: true, message: '请输入类名称', trigger: 'blur' },
  { max: 100, message: '长度在100个字符以内', trigger: 'blur' },
]

export const instName = [
  { required: true, message: '请输入实例名称', trigger: 'blur' },
  { max: 100, message: '长度在100个字符以内', trigger: 'blur' },
]

export const Subscriber = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请选择接收方'))
      } else {
        if (Array.isArray(value) && value.length == 0) {
          callback(new Error('请选择接收方'))
        }else{
          callback()
        }
      }
    },
    trigger: 'blur',
  },
]
export const instCount = [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('请输入实例数量'))
      } else {
        try {
          if (parseInt(value) > 100) {
            callback(new Error('实例最多不能超过100个'))
          } else {
            callback()
          }
        } catch (error) {
          callback(new Error('请输入数字'))
        }
      }
    },
    trigger: 'blur',
  },
]
