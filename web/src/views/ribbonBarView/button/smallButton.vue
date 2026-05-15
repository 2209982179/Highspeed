<template>
    <el-tooltip effect="light" :content="tip" placement="bottom"
        :disabled="['', null, undefined].includes(tip)" :open-after="1500">
        <button class="small_button" @click="click" :disabled="disabled || (checkDisableEditor  && $store.getters['currentDiagram'].disableEditor == true)">
            <span>
                <i :class="icon"></i>
            </span>
        </button>
    </el-tooltip>
</template>

<script>

export default {
    name: "small-button",
    props: {
        tip: "",
        icon: "",
        disabled: false
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
        click() {
            this.$emit('click');
        },
    },
};

</script>

<style lang="less">
.small_button {
    margin: 0;
    background: none;
    border: none;
    vertical-align: middle;
    padding: 0;
    padding: 4px;
    width: 48px;
    cursor: pointer;

    span {
        display: inline-block;
    }

    i {
        display: block;
        transition: color .15s linear;
        font-size: 23px;
    }
}

.small_button:hover {
    background: #B8B8B8;
}
</style>
