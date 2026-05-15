<template>
  <el-input
    ref="filterInput"
    v-model="filterKey"
    :placeholder="placeholder"
    prefix-icon="el-icon-search"
    @input="handleInput"
    @blur="handleBlur"
    @focus="isFocused = true"
    @keyup.enter.native="handleEnter"
    @mouseover.native="handleHover"
    @mouseout.native="handleHover"
    clearable
    class="filter-input"
  >
    <el-checkbox-group
      v-show="selection.length > 0 && (isFocused || isHover)"
      v-model="filterSelection"
      size="mini"
      slot="append"
      @change="handleChange"
      class="selection"
    >
      <el-checkbox-button
        v-for="item in selection"
        :label="item.label"
        :key="item.label"
        style="margin: 0; padding: 0"
        @mouseover.stop="handleHover"
        @mouseout.stop="handleHover"
      >
        {{ item.label }}
      </el-checkbox-button>
    </el-checkbox-group>
  </el-input>
</template>

<script>
  export default {
    name: 'SelectableFilter',
    props: {
      placeholder: {
        type: String,
        default: '请输入',
      },
      value: {
        type: Object,
        default: null,
      },
    },
    data() {
      return {
        filterKey: '',
        filterSelection: [],
        isFocused: false,
        isHover: false,
      }
    },
    computed: {
      selection() {
        var val = []
        if (this.value.selections) val = [...this.value.selections]
        return val
      },
    },
    watch: {
      value: {
        handler: function (newValue) {
          // 自身操作时，不响应watching
          if (this.isFocused || this.isHover) return

          var selected = []
          for (var index in this.value.selections) {
            var item = this.value.selections[index]
            if (item.checked === undefined) continue
            if (item.checked) {
              selected.push(item.label)
            }
          }
          this.filterSelection = selected
          this.filterKey = newValue.filterKey
        },
        // immediate: true, // 初始化时，不响应watching。
        // 如果开启，el-checkbox-group选项切换会在第一次使用时出现多次点击后才能响应的问题。
        deep: true,
      },
    },
    mounted() {
      this.selection.forEach((r) => {
        if (r.checked) this.filterSelection.push(r.label)
      })
    },
    methods: {
      handleChange() {
        for (var index in this.value.selections) {
          var item = this.value.selections[index]
          if (item.checked === undefined) continue
          item.checked = false
        }
        this.filterSelection.forEach((key) => {
          var f = this.value.selections.find((r) => r.label == key)
          if (f) f.checked = true
        })
        this.$emit('blur', this.value)
      },
      handleInput() {
        this.value.filterKey = this.filterKey
        this.$emit('input', this.value)
      },
      handleBlur() {
        this.isFocused = false
        this.value.filterKey = this.filterKey
        this.$emit('blur', this.value)
      },
      handleEnter() {
        this.isFocused = true
        this.value.filterKey = this.filterKey
        this.$emit('enter', this.value)
      },
      handleHover(event) {
        this.isHover =
          event?.type === 'mouseover' &&
          (event?.target?._prevClass === 'el-checkbox-button__inner' ||
            (event?.fromElement?._prevClass === 'el-checkbox-button__inner' &&
              event?.target?._prevClass === 'el-input__inner'))
      },
    },
  }
</script>

<style scoped>
  .filter-input {
    width: 100%;
  }

  .filter-input .selection {
    margin: 0;
    padding: 0;
  }

  .filter-input ::v-deep .el-input__prefix {
    height: auto;
  }

  .filter-input ::v-deep .el-input__suffix {
    height: auto;
  }

  .filter-input ::v-deep .el-input-group__append {
    padding: 0;
    display: contents;
    float: right;

    .el-checkbox-group {
      margin-top: -1px;
      position: absolute;
      top: 100%;
      z-index: 99;
    }
  }
</style>
