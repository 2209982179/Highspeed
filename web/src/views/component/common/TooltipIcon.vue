<!-- 此组件基于 el-icon el-tooltip 实现带tip的图标 -->
<template>
    <el-tooltip v-if="tip" :content="tip" effect="light" placement="bottom"
        :disabled="['', null, undefined].includes(tip)">
        <i :class="icon" @click="click" :disabled="disabled" style="cursor:pointer;"></i>
    </el-tooltip>
    <i v-else :class="icon" @click="click" :disabled="disabled" style="cursor:pointer;"></i>
</template>

<script>

export default {
    name: "TooltipIcon",
    props: {
        tip: {
            type: String,
            default: '',
        },
        icon: {
            type: String,
            default: '',
        },
        disabled: {
            type: Boolean,
            default: false,
        },
    },
    data() {
        return {
        };
    },
    methods: {
        click() {
            this.$emit('click');
        }
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
