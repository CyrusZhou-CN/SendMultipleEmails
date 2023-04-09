<script>
import { updateSendEmailSettings } from '@/api/group'
import { notifySuccess } from '@/components/iPrompt'

// 更新设置
export default {
  data() {
    return {
      isShowUpdateSettings: false,
      initSettingParams: {
        title: '设置',
        tooltip: '',
        api: updateSendEmailSettings,
        // type 可接受的值：text/password/textarea/email/search/tel/file/number/url/time/date
        fields: [
          {
            name: '_id',
            type: 'text',
            label: '发件人id',
            required: false,
            readonly: false, // 为true时会被过滤
            hidden: true
          },
          {
            name: 'email',
            type: 'email',
            label: '邮箱',
            required: true,
            readonly: true
          },
          {
            name: 'asSender',
            type: 'boolean',
            label: '用于发件',
            default: true,
            required: true
          },
          {
            name: 'smtpAddress',
            type: 'text',
            label: 'smtp服务器',
            required: true
          },
          {
            name: 'smtpPassword',
            type: 'password',
            label: 'smtp密码',
            required: true
          },
          {
            name: 'smtpProtocol',
            type: 'text',
            label: 'smtp协议',
            required: true,
            default: 'https'
          },
          {
            name: 'smtpPort',
            type: 'number',
            label: 'smtp端口',
            required: true,
            default: 465
          },
          {
            name: 'maxEmailsPerDay',
            type: 'number',
            label: '单日最大发件量',
            required: true
          }
        ]
      }
    }
  },
  methods: {
    showUpdateSettings(data) {
      // 修改初始化参数
      if (data) {
        for (const field of this.initSettingParams.fields) {
          field.default = data[field.name] || ''
        }
      }

      this.initSettingParams.title = '设置：' + data.email
      this.isShowUpdateSettings = true
    },

    // 更新后的邮件
    updatedSettings(updatingData, data) {
      // 替换原来的数据
      const index = this.data.findIndex(d => d._id === data._id)
      if (index > -1) this.data.splice(index, 1, data)

      this.isShowUpdateSettings = false
      notifySuccess('修改成功')
    }
  }
}
</script>
