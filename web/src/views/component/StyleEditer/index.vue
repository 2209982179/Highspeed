<!-- 提供样式编辑器 -->
<template>
  <div class="verticalView StyleEditer">
    <p class="MainPageMenuViewTitle">
      <span class="title">样式编辑器</span>
      <TooltipIcon tip="收起样式编辑器" icon="el-icon-d-arrow-right" class="right-icon-btn"
        @click="() => $store.commit('layout/showStyleEditer', false)"></TooltipIcon>
    </p>
    <!-- table外侧需要加div，否则无法在使用table-layout: fixed的情况下加滚动条 -->
    <div class="tablebox" v-loading="loading">
      <!-- 字体样式 -->
      <SmartFontStyle :currentDiagram="currentDiagram" :currentSelectedDiagramObjects="currentSelectedDiagramObjects"
        :currentSelectedDiagramLinks="currentSelectedDiagramLinks" />
      <!-- 图元样式 -->
      <SmartVertexStyle :currentDiagram="currentDiagram" :currentSelectedDiagramObjects="currentSelectedDiagramObjects"
        :currentSelectedDiagramLinks="currentSelectedDiagramLinks" />
      <!-- 连线样式 -->
      <SmartEdgeStyle v-show="currentDiagram.isGraph == true" :currentDiagram="currentDiagram" :currentSelectedDiagramObjects="currentSelectedDiagramObjects"
        :currentSelectedDiagramLinks="currentSelectedDiagramLinks" />
      <!-- 自定义风格 -->
      <SmartStyle :currentDiagram="currentDiagram" :currentSelectedDiagramObjects="currentSelectedDiagramObjects"
        :currentSelectedDiagramLinks="currentSelectedDiagramLinks" />
    </div>
  </div>
</template>

<script>
import SmartFontStyle from './SmartFontStyle.vue';
import SmartVertexStyle from './SmartVertexStyle.vue';
import SmartEdgeStyle from './SmartEdgeStyle.vue';
import SmartStyle from './SmartStyle.vue';

/* 批量设置样式，可撤回 */
export const batchSetStyle = (vm, cells,resetStyle) => {
  vm.currentDiagram?.graph.model.beginUpdate()
  cells.forEach(item => {
    vm.currentDiagram?.graph.model.setStyle(item, resetStyle(item))
  })
  vm.currentDiagram?.graph.model.endUpdate()
}

export default {
  components: {
    SmartFontStyle,
    SmartVertexStyle,
    SmartEdgeStyle,
    SmartStyle,
  },
  data() {
    return {
      loading: false,
    }
  },
  computed: {
    currentDiagram() {
      return this.$store.getters['currentDiagram']
    },
    currentSelectedDiagramObjects() {
      return this.$store.getters['currentSelectedDiagramObjects']
    },
    currentSelectedDiagramLinks() {
      return this.$store.getters['currentSelectedDiagramLinks']
    },
  },
  mounted() {
  },
  methods: {
  },
}
</script>
<style lang="scss" scoped>
@import 'table.scss';
@import 'index.scss';

.StyleEditer {
  position: relative;
  width: 100%;
  height: 100%;

  .tablebox {
    /* DIV高度减去标题、搜索框的高度 */
    height: calc(100% - 38px);
    overflow-y: auto;
    width: 100%;
  }
}
</style>