<template>
    <!-- 如果tip与title文本相同，认为无tip -->
    <el-tooltip v-if="tip && tip != title" :content="tip" effect="light" placement="bottom" :open-after="1500">
        <!-- 当:disabled为true时，不会触发tooltip；所以提供一个无事件的按钮 -->
        <button v-if="disabled == true" class="large_button" style="color: #999999;">
            <span>
                <i :class="icon"></i>
                <span class="icon-name">{{ title }}</span>
            </span>
        </button>
        <button v-else class="large_button" @click="click" :disabled="(checkDisableEditor  && $store.getters['currentDiagram'].disableEditor == true)">
            <span>
                <i :class="icon"></i>
                <span class="icon-name">{{ title }}</span>
            </span>
        </button>
    </el-tooltip>
    <!-- 无tip，仅显示按钮 -->
    <button v-else class="large_button" @click="click" :disabled="disabled || (checkDisableEditor  && $store.getters['currentDiagram'].disableEditor == true)">
        <span>
            <i :class="icon"></i>
            <span class="icon-name">{{ title }}</span>
        </span>
    </button>
</template>

<script>

export default {
    name: "large-button",
    props: {
        tip: [],
        icon: [],
        title: [],
        disabled: [],
    },
    data() {
        return {
            checkDisableEditor: false,
        };
    },
    mounted() {
        let vm = this
        let target = this.$el
        while (target && target.nodeType === Node.ELEMENT_NODE) {
            if (Array.from(target.classList).join(' ').includes('ribbonBar-tab-pane-special')) {
                vm.checkDisableEditor = true
                return
            }
            target = target.parentElement; // 获取父元素
        }
    },
    methods: {
        click() {
            this.$emit('click');
        },
    },
};

</script>

<style lang="less">
.large_button {
    margin: 0;
    background: none;
    border: none;
    vertical-align: middle;
    min-width: 70px;
    padding: 0;
    padding: 4px;
    cursor: pointer;

    span {
        display: inline-block;
    }

    i {
        display: block;
        transition: color .15s linear;
        font-size: 32px;
        line-height: 32px;
        height: 32px;
        padding-bottom: 8px;
    }

    .icon-name {
        display: inline-block;
        font-size: 12px;
        line-height: 12px;
        height: 12px;
    }
}

.large_button:hover {
    background: #B8B8B8;
}
</style>
