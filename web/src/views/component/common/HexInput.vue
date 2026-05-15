<template>
  <div>
    <el-input
      v-model="hexValue"
      :placeholder="placeholder"
      :disabled="disabled"
      :maxlength="length + 2"
      @input="handleInput"
      @blur="handleBlur"
    >
      <template #prefix><span class="hexPrefix">0x</span></template>
    </el-input>
  </div>
</template>

<script>
  export default {
    name: 'HexInput',
    props: {
      value: {
        type: String,
        default: '0x',
      },
      disabled: {
        type: Boolean,
        default: false,
      },
      length: {
        type: Number,
        default: 4,
      },
      placeholder: {
        type: String,
        default: '',
      },
    },
    computed: {
      hexValue: {
        get() {
          return this.processHex(this.value)
        },
        set(val) {
          // 移除0x前缀，只保留16进制部分
          const hexPart = this.processHex(val)
          // 验证是否为有效的16进制字符串
          if (/^[0-9A-F]*$/.test(hexPart) && hexPart.length <= this.length) {
            this.$emit('input', `0x${hexPart}`)
          } else {
            this.$emit('input', this.value)
          }
        },
      },
    },
    methods: {
      handleInput(val) {
        // 处理输入事件，确保输入符合要求
        this.hexValue = val
      },
      handleBlur() {
        this.$emit('blur', this.validate(this.value))
      },
      validate(val) {
        const hexPart = this.processHex(val)
        const isValid =
          !/^0x[0-9A-F]*$/.test(hexPart) || hexPart.length > this.length
        if (!isValid) {
          return new Error('请输入有效的十六进制值')
        }
        this.$emit('input', `0x${hexPart}`)
        return `0x${hexPart}`
      },
      processHex(val) {
        val = val.replace(/^0x/i, '')
        if (!val) return ''
        val = val.padStart(this.length, '0') // 用0补全长度
        if (val.length > this.length) val = val.replace(/^0+/, '') // 如果超出长度，去除前部的0
        return val.toUpperCase()
      },
    },
  }
</script>

<style scoped>
  .hexPrefix {
    display: flex;
    align-items: center; /* 垂直居中 */
    justify-content: center; /* 水平居中 */
    height: 100%;
    margin-left: 12px; /* 调整0x前缀的位置 */
  }
</style>
