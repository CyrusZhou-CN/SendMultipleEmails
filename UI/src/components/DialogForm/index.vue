<template>
  <q-card class="q-pa-sm" style="width: 400px">
    <div class="text-subtitle1 q-mb-sm">{{ initParams.title }}</div>

    <div class="column q-gutter-sm" style="margin: 10px; margin-right: 25px">
      <template v-for="field in fields">
        <template v-if="field.isSlider">
          <div :key="'slider_' + field.name" class="text-subtitle1 q-mb-lg">
            {{ field.label }}
            <q-tooltip v-if="field.tooltip">{{ field.tooltip }}</q-tooltip>
          </div>
          <q-slider
            :key="field.name"
            v-model="data[field.name]"
            :min="field.min || 0"
            :max="field.max || 500"
            :step="field.step || 10"
            label
            label-always
            :label-value="data[field.name] ? data[field.name] : $t('unlimited')"
            style="min-width: 300px"
          />
        </template>
        <template v-else>
          <q-input
            :key="field.name"
            v-model="data[field.name]"
            clearable
            clear-icon="close"
            outlined
            :type="field.type"
            :label="field.label"
            dense
            :readonly="field.readonly"
          />
        </template>
      </template>

      <div class="row justify-end q-gutter-sm">
        <q-btn
          v-close-popup
          :size="btn_cancel.size"
          :color="btn_cancel.color"
          :label="btn_cancel.label"
          :dense="btn_cancel.dense"
        />

        <q-btn
          :size="btn_confirm.size"
          :color="btn_confirm.color"
          :label="btn_confirm.label"
          :dense="btn_confirm.dense"
          @click="confirm"
        />
      </div>
    </div>
  </q-card>
</template>

<script>
/**
 * 说明
 * 该模块为通用的增加和修改模块
 */

import { button } from '@/themes/index'
import { notifyError } from '@/components/iPrompt'

import _ from 'lodash'

export default {
  props: {
    initParams: {
      type: Object,
      require: true,
      default: null
    },

    // 类型，有 create,update 两种
    type: {
      type: String,
      default: 'create'
    },

    initData: {
      type: Object,
      default: null
    }
  },

  data() {
    return {
      data: {}
    }
  },

  computed: {
    btn_confirm() {
      return button.btn_confirm
    },
    btn_cancel() {
      return button.btn_cancel
    },
    fields() {
      const fields = this.initParams.fields.filter(f => !f.hidden)
      console.log('fields:', fields)
      return fields
    },

    isUpdate() {
      return this.type === 'update'
    }
  },
  created() {
    console.log('iform create data:', this.initParams)
    // 处理默认数据
    for (const field of this.initParams.fields) {
      if (field.default) {
        // 判断是否是方法
        if (typeof field.default === 'function') {
          this.$set(this.data, field.name, field.default())
        } else {
          this.$set(this.data, field.name, field.default)
        }
      }
    }
  },

  mounted() {},

  methods: {
    async confirm() {
      // 判断数据的必要性
      for (const field of this.fields) {
        if (field.required && !this.data[field.name] && !field.isSlider) {
          notifyError(`${field.label} ${this.$t('isRequired')}`)
          return
        }
      }

      if (!this.initParams.interceptApi && !this.initParams.api) {
        throw new Error(this.$t('api_required'))
      }

      const result = await this[`${this.type}Doc`]()

      if (result.code !== 200) {
        notifyError(result.message)
      }
    },

    // 新建
    async createDoc() {
      const createData = _.cloneDeep(this.data)

      if (this.initParams.handler && this.initParams.handler.before) {
        if (!this.initParams.handler.before(createData)) {
          return {
            code: 200,
            message: this.$t('before_handler_failed')
          }
        }
      }

      console.log('createDoc data:', createData)

      // 拦截api响应，直接返回数据
      if (this.initParams.interceptApi) {
        this.$emit(`${this.type}Success`, this.createData)
        return {
          code: 200,
          message: this.$t('intercept_api_success')
        }
      }

      const result = await this.initParams.api(createData)

      // 如果成功了，则提示
      if (result.code === 200) {
        // 添加完成后，清空数据
        this.data = {}

        // 关闭窗体
        this.$emit(`${this.type}Success`, result.data)
      }

      return result
    },

    // 更新
    async updateDoc() {
      // 判断是否有 _id,如果没有，不进行更新
      if (!this.data._id && !this.data.id) {
        return {
          code: 204,
          message: this.$t('no_id_found')
        }
      }

      // 整理更新的数据
      const updateData = {}
      this.initParams.fields.forEach(field => {
        if (!field.readonly) {
          updateData[field.name] = this.data[field.name]
        }
      })

      // 前置操作
      if (this.initParams.handler && this.initParams.handler.before) {
        if (!this.initParams.handler.before(updateData)) {
          return {
            code: 200,
            message: this.$t('before_handler_failed')
          }
        }
      }

      // 拦截api响应，直接返回数据
      if (this.initParams.interceptApi) {
        this.$emit(
          `${this.type}Success`,
          Object.assign({}, this.data, updateData)
        )
        return {
          code: 200,
          message: this.$t('intercept_api_success')
        }
      }

      // console.log('updatedoc:', updateData, this.data)
      const result = await this.initParams.api(
        updateData._id || updateData.id,
        updateData
      )

      // 如果成功了，则提示
      if (result.code === 200) {
        // console.log(`${this.type}Success`, updateData)

        // 关闭窗体
        this.$emit(`${this.type}Success`, updateData, result.data)

        // 添加完成后，清空数据
        this.data = {}
      }

      return result
    }
  }
}
</script>

<style lang="scss">
.q-slider--no-value {
  .q-slider__thumb {
    opacity: 1 !important;
  }
}
</style>
