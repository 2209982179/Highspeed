<!-- 字体样式 -->
<template>
    <table cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="group1 unselectable" style="background-color: #eee;" colspan="10"
                    @click="() => { isShow = !isShow }">
                    <i class="el-icon-edit" />
                    字体样式
                    <i :class="{ 'el-icon-arrow-down': isShow, 'el-icon-arrow-up': !isShow }" style="float: right;" />
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px">
                <td class="unselectable" colspan="3" style="border-bottom: 0;">字体</td>
                <td colspan="7" style="border-bottom: 0;" class="value unselectable">
                    <div style="display: block; float: left;">
                        <el-color-picker class="font-color-picker" data-label="A" :style="`color: ${fontColor};`"
                            :show-alpha="false" :predefine="fontColors" v-model="fontColor"
                            @active-change="(val) => { fontColor = val }" @change="handlerFontColor">
                        </el-color-picker>
                    </div>
                    <div class="stroke-size-input" style="display: block;float: left;margin-left: 6px;">
                        <!-- <el-input-number controls-position="right" :min="1" :max="99" data-unit="pt"
                            class="unit-el-input-number" v-model="fontSize" @change="handlerFontSize" /> -->
                        <el-select placeholder="字体大小" filterable allow-create default-first-option style="width: 54px;"
                            ref="searchFontSize" v-model="fontSize" @change="handlerFontSize"
                            @input.native="filterFontSize">
                            <el-option v-for="(item, index) in [12, 14, 16, 18, 20, 22, 24, 26, 28, 32, 48]"
                                :key="index" :label="item" :value="item"></el-option>
                        </el-select>
                    </div>
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px">
                <td class="unselectable" colspan="3"></td>
                <td colspan="7" class="value unselectable">
                    <el-tooltip content="字体加粗" effect="light" placement="bottom">
                        <div :class="{ style_btn: true, checked: isBold }" @click="handlerBold">
                            <div class="context">
                                <span>B</span>
                            </div>
                        </div>
                    </el-tooltip>
                    <el-tooltip content="斜体字" effect="light" placement="bottom">
                        <div style="margin-left: 6px;" :class="{ style_btn: true, checked: isItalic }"
                            @click="handlerItalic">
                            <div class="context">
                                <span style="font-style: italic;">I</span>
                            </div>
                        </div>
                    </el-tooltip>
                    <el-tooltip content="下划线" effect="light" placement="bottom">
                        <div style="margin-left: 6px;" :class="{ style_btn: true, checked: isUnderline }"
                            @click="handlerUnderline">
                            <div class="context">
                                <span style="text-decoration: underline;">U</span>
                            </div>
                        </div>
                    </el-tooltip>
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px">
                <td class="unselectable" colspan="3" style="border-bottom: 0;">对齐方式</td>
                <td colspan="7" style="border-bottom: 0;" class="value unselectable">
                    <el-tooltip content="左对齐" effect="light" placement="bottom">
                        <div :class="{ style_btn: true, checked: HorizontalAlign == 'left' }"
                            @click="handlerHorizontalAlign('left')">
                            <div class="context">
                                <i class="elemnt-icons el-icon-a-6zuoduiqi-tubiao" style="font-size: 12px;"></i>
                            </div>
                        </div>
                    </el-tooltip>
                    <el-tooltip content="水平居中" effect="light" placement="bottom">
                        <div style="margin-left: 6px;" :class="{ style_btn: true, checked: HorizontalAlign == 'center' }"
                            @click="handlerHorizontalAlign('center')">
                            <div class="context">
                                <i class="elemnt-icons el-icon-a-7juzhongduiqi-tubiao" style="font-size: 12px;"></i>
                            </div>
                        </div>
                    </el-tooltip>
                    <el-tooltip content="右对齐" effect="light" placement="bottom">
                        <div style="margin-left: 6px;" :class="{ style_btn: true, checked: HorizontalAlign == 'right' }"
                            @click="handlerHorizontalAlign('right')">
                            <div class="context">
                                <i class="elemnt-icons el-icon-a-8youduiqi-tubiao" style="font-size: 12px;"></i>
                            </div>
                        </div>
                    </el-tooltip>
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px">
                <td class="unselectable" colspan="3"></td>
                <td colspan="7" class="value unselectable">
                    <el-tooltip content="向上对齐" effect="light" placement="bottom">
                        <div :class="{ style_btn: true, checked: VerticalAlign == 'top' }"
                            @click="handlerVerticalAlign('top')">
                            <div class="context">
                                <i class="el-icon-download" style="font-size: 12px;transform: rotateX(180deg);"></i>
                            </div>
                        </div>
                    </el-tooltip>
                    <el-tooltip content="垂直居中" effect="light" placement="bottom">
                        <div style="margin-left: 6px;" :class="{ style_btn: true, checked: VerticalAlign == 'center' }"
                            @click="handlerVerticalAlign('center')">
                            <div class="context">
                                <i class="el-icon-sort" style="font-size: 12px;transform: rotate(90deg);"></i>
                            </div>
                        </div>
                    </el-tooltip>
                    <el-tooltip content="向下对齐" effect="light" placement="bottom">
                        <div style="margin-left: 6px;" :class="{ style_btn: true, checked: VerticalAlign == 'bottom' }"
                            @click="handlerVerticalAlign('bottom')">
                            <div class="context">
                                <i class="el-icon-download" style="font-size: 12px;"></i>
                            </div>
                        </div>
                    </el-tooltip>
                </td>
            </tr>
            <tr v-show="isShow" style="height: 32px">
                <td class="unselectable" colspan="3">文本位置</td>
                <td colspan="7" class="value unselectable">
                    <div class="stroke-size-input">
                        <el-select placeholder="文本位置" style="width: 80px;" v-model="selectedLabelPosition"
                            @change="handlerLabelPosition">
                            <el-option v-for="(item, index) in LabelPositions" :key="index" :label="item"
                                :value="item"></el-option>
                        </el-select>
                    </div>
                </td>
            </tr>
            <!-- <tr v-show="isShow" style="height: 32px">
                <td class="unselectable" colspan="3">书写方向</td>
                <td colspan="7" class="value unselectable">
                    <div class="stroke-size-input">
                        <el-select placeholder="书写方向" style="width: 80px;" v-model="selectedTextDirection"
                            @change="handlerTextDirection">
                            <el-option v-for="(item, index) in TextDirections" :key="index" :label="item"
                                :value="item"></el-option>
                        </el-select>
                    </div>
                </td>
            </tr> -->
            <!-- <tr v-show="isShow" style="height: 32px">
                <td colspan="10" class="value unselectable" style="font-size: 12px;">
                    <el-checkbox v-model="isWrap" @change="handlerIsWrap">自动换行</el-checkbox>
                </td>
            </tr> -->
        </tbody>
    </table>
</template>

<script>
import mxgraph from '@/mxgraph/graph'
const { mxUtils } = mxgraph
import { batchSetStyle } from './index.vue';
export default {
    props: {
        currentDiagram: null,
        currentSelectedDiagramLinks: null,
        currentSelectedDiagramObjects: null,
    },
    data() {
        return {
            isShow: true,
            fontSize: 12,
            fontColor: '#774400',
            fontColors: ['#666', '#eee', '#ccc', '#222', '#000', '#fff'],
            selectedLabelPosition: '居中',
            LabelPositions: ['居中', '上', '下', '左', '右'],
            selectedTextDirection: '自动',
            TextDirections: ['自动', '垂直'],
            isWrap: false,
            isBold: false,
            isItalic: false,
            isUnderline: false,
            HorizontalAlign: '',
            VerticalAlign: '',
        }
    },
    methods: {
        /* 修改文本颜色 */
        handlerFontColor() {
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'fontColor', this.fontColor)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        /* 修改文本大小 */
        handlerFontSize() {
            let resetStyle = item => {
                let style = item.style
                let number = parseInt(this.fontSize, 10);
                if (isNaN(number)) {
                    number = 12;
                } else if (number < 12) {
                    number = 12;
                } else if (number > 99) {
                    number = 99;
                }
                this.fontSize = number.toString()
                style = mxUtils.setStyle(style, 'fontSize', number)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        filterFontSize() {
            // 只能输入数字
            let str = this.$refs.searchFontSize.$data.selectedLabel.replace(/[^0-9]/g, '');
            if (str == '') {
                this.$refs.searchFontSize.$data.selectedLabel = str
            } else {
                // 确保数字在12到99之间，因为最小值是12
                let number = parseInt(str, 10);
                if (number > 99) {
                    number = 99;
                }
                this.$refs.searchFontSize.$data.selectedLabel = number.toString();
            }
        },
        /* 修改文本位置 */
        handlerLabelPosition() {
            let resetStyle = item => {
                let style = item.style
                if (this.selectedLabelPosition == '居中') {
                    style = mxUtils.setStyle(style, 'labelPosition', null)
                    style = mxUtils.setStyle(style, 'verticalLabelPosition', null)
                    style = mxUtils.setStyle(style, 'align', null)
                    style = mxUtils.setStyle(style, 'verticalAlign', null)
                }
                else if (this.selectedLabelPosition == '上') {
                    style = mxUtils.setStyle(style, 'labelPosition', 'center')
                    style = mxUtils.setStyle(style, 'verticalLabelPosition', 'top')
                    style = mxUtils.setStyle(style, 'align', 'center')
                    style = mxUtils.setStyle(style, 'verticalAlign', 'bottom')
                }
                else if (this.selectedLabelPosition == '下') {
                    style = mxUtils.setStyle(style, 'labelPosition', 'center')
                    style = mxUtils.setStyle(style, 'verticalLabelPosition', 'bottom')
                    style = mxUtils.setStyle(style, 'align', 'center')
                    style = mxUtils.setStyle(style, 'verticalAlign', 'top')
                }
                else if (this.selectedLabelPosition == '左') {
                    style = mxUtils.setStyle(style, 'labelPosition', 'left')
                    style = mxUtils.setStyle(style, 'verticalLabelPosition', 'middle')
                    style = mxUtils.setStyle(style, 'align', 'right')
                    style = mxUtils.setStyle(style, 'verticalAlign', 'middle')
                }
                else if (this.selectedLabelPosition == '右') {
                    style = mxUtils.setStyle(style, 'labelPosition', 'right')
                    style = mxUtils.setStyle(style, 'verticalLabelPosition', 'middle')
                    style = mxUtils.setStyle(style, 'align', 'left')
                    style = mxUtils.setStyle(style, 'verticalAlign', 'middle')
                }
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        /* 修改书写方向 */
        handlerTextDirection() {
            let resetStyle = item => {
                let style = item.style
                if (this.selectedTextDirection == '自动') {
                    style = mxUtils.setStyle(style, 'textDirection', null)
                }
                else if (this.selectedTextDirection == '垂直') {
                    style = mxUtils.setStyle(style, 'textDirection', 'vertical-lr')
                }
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        /* 自动换行 */
        handlerIsWrap(val) {
            console.log('handlerIsWrap', val);
        },
        /* 文本水平方向 */
        handlerHorizontalAlign(val) {
            this.HorizontalAlign = val;
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'align', val)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        /* 文本垂直方向 */
        handlerVerticalAlign(val) {
            this.VerticalAlign = val;
            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'verticalAlign', val)
                if (item.Stereotype) {
                    if (val == 'top') {
                        style = mxUtils.setStyle(style, 'spacingTop', 13)
                    } else {
                        style = mxUtils.setStyle(style, 'spacingTop', 0)
                    }
                }
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
        /* 文本加粗 */
        handlerBold() {
            this.isBold = !this.isBold
            this.setFontStyle()
        },
        /* 斜体字 */
        handlerItalic() {
            this.isItalic = !this.isItalic;
            this.setFontStyle()
        },
        /* 下划线 */
        handlerUnderline() {
            this.isUnderline = !this.isUnderline;
            this.setFontStyle()
        },
        setFontStyle() {
            let fontStyle = null
            if (!this.isBold && !this.isItalic && !this.isUnderline) {
                fontStyle = null
            }
            else if (this.isBold && !this.isItalic && !this.isUnderline) {
                fontStyle = 1
            }
            else if (!this.isBold && this.isItalic && !this.isUnderline) {
                fontStyle = 2
            }
            else if (this.isBold && this.isItalic && !this.isUnderline) {
                fontStyle = 3
            }
            else if (!this.isBold && !this.isItalic && this.isUnderline) {
                fontStyle = 4
            }
            else if (this.isBold && !this.isItalic && this.isUnderline) {
                fontStyle = 5
            }
            else if (!this.isBold && this.isItalic && this.isUnderline) {
                fontStyle = 6
            }
            else if (this.isBold && this.isItalic && this.isUnderline) {
                fontStyle = 7
            }

            let resetStyle = item => {
                let style = item.style
                style = mxUtils.setStyle(style, 'fontStyle', fontStyle)
                return style
            }
            batchSetStyle(this, this.currentSelectedDiagramObjects, resetStyle)
        },
    },
}
</script>
<style lang="scss" scoped>
@import 'table.scss';
@import 'index.scss';
</style>