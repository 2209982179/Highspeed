<template>
    <el-tooltip effect="light" :content="tip" placement="bottom" :disabled="['', null, undefined].includes(tip)"
        :open-after="1500">
        <el-dropdown :disabled="disabled || (checkDisableEditor  && $store.getters['currentDiagram'].disableEditor == true)" @command="command">
            <button class="ribbon-dropdown">
                <span>
                    <i :class="icon"></i>
                    <span class="icon-name">
                        {{ title }}
                        <i class="el-icon-caret-bottom"></i>
                    </span>
                </span>
            </button>
            <el-dropdown-menu slot="dropdown">
                <!-- 标题与tip相同时，不提供tip -->
                <el-tooltip v-for="(item, index) in menuItem" :key="index" :content="item.tip" placement="right" effect="light">
                    <!-- 当:disabled为true时，不会触发tooltip；所以提供一个无事件的按钮 -->
                    <el-dropdown-item v-if="item.disabled == true" :icon="item.icon" style="color: #999999;">
                        {{ item.title }}
                    </el-dropdown-item>
                    <el-dropdown-item v-else @click.native="item.click" :icon="item.icon" :disabled="(checkDisableEditor  && $store.getters['currentDiagram'].disableEditor == true)">
                        {{ item.title }}
                    </el-dropdown-item>
                </el-tooltip>
            </el-dropdown-menu>
        </el-dropdown>

    </el-tooltip>
</template>

<script>

export default {
    name: "large-dropdown-button",
    props: {
        tip: "",
        icon: "",
        title: "",
        disabled: false,
        // {tip: "",click: () => { },disabled: false,title: "",icon: ""}
        menuItem: Array,
    },
    setup(props) {
        return { props };
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
            if (Array.from(target.classList).join(' ')?.includes('ribbonBar-tab-pane-special')) {
                vm.checkDisableEditor = true
                return
            }
            target = target.parentElement; // 获取父元素
        }
    },
    methods: {
        command() {
            this.$emit('command');
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
