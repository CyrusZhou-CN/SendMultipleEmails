<script>
import { modifyEmail } from '@/api/group'
import { notifySuccess } from '@/components/iPrompt'

const emailCommonInfo = [
  {
    name: '_id',
    type: 'text',
    label: '邮箱Id',
    required: true,
    readonly: false,
    hidden: true
  },
  {
    name: 'email',
    type: 'email',
    label: '邮箱',
    required: true
  },
  {
    name: 'description',
    type: 'text',
    label: '描述',
    required: true
  }
]

export default {
  data() {
    return {
      isShowModifyEmailDialog: false,
      initModifyEmailParams: {
        title: '修改',
        tooltip: '',
        api: modifyEmail,
        // type 可接受的值：text/password/textarea/email/search/tel/file/number/url/time/date
        fields: []
      }
    }
  },

  computed: {
    modifyEmailTitle() {
      if (this.group.groupType === 1) {
        return '修改发件箱'
      }
      return '修改收件箱'
    }
  },

  methods: {
    showModifyEmailDialog(data) {
      const fields = [...emailCommonInfo]
      this.initModifyEmailParams.fields = fields
      this.initModifyEmailParams.title = this.modifyEmailTitle

      for (const field of fields) {
        // 设置默认值
        field.default = data[field.name]
      }

      this.isShowModifyEmailDialog = true
    },

    modifiedEmail(data) {
      console.log('modifiedEmail:', data)
      // 替换原来的数据
      const index = this.data.findIndex(d => d._id === data._id)
      if (index > -1) this.data.splice(index, 1, data)

      this.isShowModifyEmailDialog = false
      notifySuccess('修改成功')
    }
  }
}
</script>
