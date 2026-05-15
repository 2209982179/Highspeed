<template>
  <!-- 打开弹框的动画 -->
  <transition name="animation">
    <div
      class="loadindWrap"
      v-if="showLoading"
      :style="{ background: backgroundColor }"
    >
      <div class="loadingContent">
        <div class="iBox">
          <i class="el-icon-loading"></i>
        </div>
        <div class="text">{{ text }}</div>
        <div style="margin-top: 10px;">
          <el-button v-if="cancancel" @click="cancel" >关闭加载页面</el-button>
        </div>
      </div>
    </div>
  </transition>
</template>

<script>
export default {
  data() {
    return {
      showLoading: false, // 控制显示与隐藏的标识
      backgroundColor: "rgba(0, 0, 0, 0.5)", // 默认背景色
      text: "加载中...", // 默认文字
      timeout:30,//默认30秒之后可以手动关闭loading
      cancancel:false,
    };
  },
  methods:{
    cancel(){
      this.$hideLoading()
      if(this.cancelAction) this.cancelAction()
    },
  }
};
</script>

<style lang="scss" scoped>
.loadindWrap {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  z-index: 99999;
  justify-content: center;
  align-items: center;
  .loadingContent {
    color: #fff;
    text-align: center;
    .iBox {
      margin-bottom: 6px;
      i {
        font-size: 20px;
        color: #fff;
      }
    }
  }
}
// 加一个过渡效果
.animation-enter, .animation-leave-to { opacity: 0;}
.animation-enter-active, .animation-leave-active { transition: opacity 0.6s; }
</style>