<script>
import { modifyEmail } from '@/api/group'
import { notifySuccess } from '@/components/iPrompt'

export default {
  data() {
    return {
      isShowModifyEmailDialog: false,
      initModifyEmailParams: {
        title: this.$t('modify'),
        tooltip: '',
        api: modifyEmail,
        // type 可接受的值：text/password/textarea/email/search/tel/file/number/url/time/date
        fields: []
      }
    }
  },

  computed: {
    modifyEmailTitle() {
      if (this.group.groupType === 'send') {
        return this.$t('modifyOutbox')
      }
      return this.$t('modifyInbox')
    }
  },

  methods: {
    showModifyEmailDialog(data) {
      const fields = [
        {
          name: '_id',
          type: 'text',
          label: this.$t('table.emailId'),
          required: true,
          readonly: false,
          hidden: true
        },
        {
          name: 'userName',
          type: 'text',
          label: this.$t('table.userName'),
          required: true
        },
        {
          name: 'email',
          type: 'email',
          label: this.$t('table.email'),
          required: true
        }
      ]

      const emailSender = [
        {
          name: 'aliasEmail',
          type: 'text',
          label: this.$t('table.aliasEmail')
        },
        {
          name: 'smtp',
          type: 'text',
          label: this.$t('table.smtp'),
          required: true
        },
        {
          name: 'port',
          type: 'number',
          label: this.$t('table.port'),
          required: true,
          default: '587'
        },
        {
          name: 'enableSsl',
          type: 'checkbox',
          label: this.$t('table.enableSsl'),
          required: true,
          default: true
        },
        {
          name: 'password',
          type: 'password',
          label: this.$t('table.password'),
          required: true
        }
      ]
      if (this.group.groupType === 'send') {
        fields.push(...emailSender)
      }
      fields[0].default = this.group._id
      this.initModifyEmailParams.fields = fields
      this.initModifyEmailParams.title = this.modifyEmailTitle

      // 修改初始化参数
      if (data) {
        this.$set(
          this.initModifyEmailParams.fields[0],
          'default',
          data._id || ''
        )
        this.$set(
          this.initModifyEmailParams.fields[1],
          'default',
          data.userName || ''
        )
        this.$set(
          this.initModifyEmailParams.fields[2],
          'default',
          data.email || ''
        )
      }
      if (this.group.groupType === 'send') {
        this.$set(
          this.initModifyEmailParams.fields[3],
          'default',
          data.aliasEmail || ''
        )
        this.$set(
          this.initModifyEmailParams.fields[4],
          'default',
          data.smtp || ''
        )
        this.$set(
          this.initModifyEmailParams.fields[5],
          'default',
          data.port || ''
        )
        this.$set(
          this.initModifyEmailParams.fields[6],
          'default',
          data.enableSsl || false
        )
        this.$set(
          this.initModifyEmailParams.fields[7],
          'default',
          data.password || ''
        )
      }

      this.isShowModifyEmailDialog = true
    },

    modifiedEmail(data) {
      console.log('modifiedEmail:', data)
      // 替换原来的数据
      const index = this.data.findIndex(d => d._id === data._id)
      if (index > -1) this.data.splice(index, 1, data)

      this.isShowModifyEmailDialog = false
      notifySuccess(this.$t('editSuccess'))
    }
  }
}
</script>
