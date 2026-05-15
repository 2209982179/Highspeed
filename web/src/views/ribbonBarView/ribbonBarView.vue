<!-- 
  导航栏菜单插件；
  根据 current Diagram 改变page显隐；
  根据 current Diagram selected object 改变按钮可用状态；
 -->
<template>
  <el-tabs id="ribbonBar" v-model="activeName" :stretch="false">
    <el-tab-pane label="开始" name="Start">
      <div class="content">
        <div class="group first">
          <div class="buttons">
            <LargeButton
              title="测试用例"
              tip="测试用例"
              icon="el-icon-plus"
              @click="ShowTestCaseList"
            ></LargeButton>
            <LargeButton
              title="更新日志"
              tip="更新日志"
              icon="el-icon-plus"
              @click="Showchangelog"
            ></LargeButton>
          </div>
          <div class="label">
            <span>数据管理</span>
          </div>
        </div>
      </div>
    </el-tab-pane>
  </el-tabs>
</template>
<script>
  import LargeButton from '../ribbonBarView/button/largeButton.vue'
  import UpdateLogDialog from '../about/changelog.vue'
  import TestCaseList from '../business/TestCaseList/TestCaseList.vue'
  export default {
    data() {
      return {
        activeName: 'Start',
        portShow: false,
        checkedValues: [],
      }
    },
    components: {
      LargeButton,
    },
    created() {
      this.ProjectType = this.$route.query.projecttype
    },
    computed: {
      currentShowViews: {
        get() {
          return this.$store.getters['currentShowViews']
        },
      },
      currentDiagram() {
        return this.$store.getters['currentDiagram']
      },
    },
    watch: {},
    mounted() {
      this.$GlobalContext.ribbonBar = this
    },
    methods: {
      ShowTestCaseList() {
        this.$showDialog(TestCaseList, {})
      },
      Showchangelog() {
        this.$showDialog(UpdateLogDialog, {})
      },
    },
  }
</script>

<!-- el-tabs 样式，需要采用 deep -->
<style lang="scss" scoped>
  @import './ribbonBar-el-tabs.scss';
</style>

<!-- el-tab-pane 内容的样式是需要传递的，不能使用scoped -->
<style lang="less">
  @import './ribbonBar-group.scss';
</style>
