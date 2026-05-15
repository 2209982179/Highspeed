<template>
  <el-dialog
    class="message-box"
    :key="Math.random()"
    :title="title"
    :visible.sync="dialogVisible"
    :before-close="onBeforeClose"
    width="420px"
  >
    <div style="position: relative">
      <div
        :class="iconClassName"
        style="
          font-size: 24px;
          position: absolute;
          top: 50%;
          transform: translateY(-50%);
        "
      ></div>
      <div
        style="display: inline-block; padding-left: 36px; padding-right: 12px"
      >
        <p>{{ message }}</p>
      </div>
    </div>
    <span slot="footer" class="dialog-footer">
      <el-button
        v-for="btn in buttons"
        :type="btn.type"
        :key="btn.text"
        size="small"
        @click="
          () => {
            let result = btn.onClick ? btn.onClick() : _onClick()
            if ([true, null, undefined].includes(result)) dialogVisible = false
          }
        "
      >
        {{ btn.text }}
      </el-button>
    </span>
  </el-dialog>
</template>

<script>
  export default {
    name: 'message-box',
    components: {},
    props: {
      parentObj: Object,
    },
    data() {
      return {
        dialogVisible: false,
        message: '',
        type: '',
        buttons: [
          {
            text: '',
            type: 'primary',
            onClick: () => {},
            visible: true,
          },
        ],
        iconClassName: '',
        title: '',
        close: () => {},
      }
    },
    created() {
      this.statusInit()
      this.buttons = this.buttons.filter((x) =>
        [null, undefined, true].includes(x.visible)
      )
    },
    mounted() {},
    watch: {},
    computed: {},
    methods: {
      statusInit() {
        let className = ''
        let _title = ''
        switch (this.type) {
          case 'warning':
            className = 'el-icon-warning'
            _title = '警告'
            break
          case 'success':
            className = 'el-icon-success'
            _title = '成功'
            break
          case 'info':
            className = 'el-icon-info'
            _title = '提示'
            break
          case 'error':
            className = 'el-icon-error'
            _title = '错误'
            break
          default:
            className = 'el-icon-info'
            _title = '提示'
        }

        this.iconClassName = className
        if (!this.title) {
          this.title = _title
        }
      },
      _onClick() {
        this.dialogVisible = false
      },
      onBeforeClose(done) {
        if (this.close) this.close()
        done()
      },
    },
  }
</script>

<style lang="scss">
  .message-box {
    .el-dialog {
      position: absolute;
      border-radius: 4px;
      top: 30%;
    }

    .el-dialog__title {
      padding-left: 0;
      margin-bottom: 0;
      font-size: 18px;
      line-height: 1;
    }

    .el-dialog__header {
      padding: 15px 15px 10px;
    }

    .el-dialog__body {
      padding: 10px 15px;
    }

    .el-dialog__footer {
      padding: 5px 15px 10px;
    }

    .el-icon-warning {
      color: #e6a23c;
    }

    .el-icon-success {
      color: #67c23a;
    }

    .el-icon-info {
      color: #6397ff;
    }

    .el-icon-error {
      color: #f56c6c;
    }
  }
</style>
