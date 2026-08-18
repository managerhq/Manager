using ManagerServer.Orm;
using System;
using System.Linq;

namespace ManagerServer
{
    public partial class ApplicationData
    {
        [Table("Objects", WithoutRowId = true)]
        public class Object
        {
            [PrimaryKey] public Guid Key { get; set; }
            [Indexed] public Guid ContentType { get; set; }
            public byte[] Content { get; set; }
            public long Timestamp { get; set; }

            public static Object From(ManagerServer.Model.Object o)
            {
                var o2 = Serialization.Serialize(o);
                return new Object()
                {
                    Key = o.Key,
                    Timestamp = o.Timestamp,
                    ContentType = o2.Item1,
                    Content = o2.Item2
                };
            }

            public bool Equals2(Object obj)
            {
                if (obj == null) return false;
                if (Key != obj.Key) return false;
                if (ContentType != obj.ContentType) return false;
                if (Content != null && obj.Content == null) return false;
                if (Content == null && obj.Content != null) return false;
                if (!Content.SequenceEqual(obj.Content)) return false;
                return true;
            }
        }

        [Table("Images", WithoutRowId = true)]
        public class Image
        {
            [PrimaryKey] public Guid Key { get; set; }
            public string ContentType { get; set; }
            public byte[] Content { get; set; }
            public long Timestamp { get; set; }
        }

        [Table("Blobs", WithoutRowId = true)]
        public class Blob
        {
            [PrimaryKey] public Guid Key { get; set; }
            public string Name { get; set; }
            public string ContentType { get; set; }
            public byte[] Content { get; set; }
        }

        [Table("Changes", WithoutRowId = true)]
        public class Change
        {
            [PrimaryKey] public Guid Key { get; set; }
            [Indexed] public Guid Commit { get; set; }
            [Indexed] public Guid Object { get; set; }
            [Indexed] public string User { get; set; }
            [Indexed] public long Timestamp { get; set; }
            [Indexed] public Guid ContentTypeBefore { get; set; }
            [Indexed] public Guid ContentTypeAfter { get; set; }
            public byte[] ContentBefore { get; set; }
            public byte[] ContentAfter { get; set; }

            public bool IsCreatingChange { get { return ContentTypeBefore == Guid.Empty && ContentTypeAfter != Guid.Empty; } }
            public bool IsUpdatingChange { get { return ContentTypeBefore != Guid.Empty && ContentTypeAfter != Guid.Empty; } }
            public bool IsDeletingChange { get { return ContentTypeBefore != Guid.Empty && ContentTypeAfter == Guid.Empty; } }

            [ManagerServer.Model.Attributes.TableColumn] public DateTime GetTimestamp() => new DateTime(Timestamp);
            [ManagerServer.Model.Attributes.TableColumn] public string GetUser() => User;
            [ManagerServer.Model.Attributes.TableColumn] public string GetDescription()
            {
                var before = string.Empty;
                if (ContentTypeBefore != Guid.Empty)
                {
                    var o = Serialization.Deserialize(ContentTypeBefore, ContentBefore);
                    if (o != null)
                    {
                        before = ManagerServer.Globalization.Strings.GetPropertyValue(o.GetType().Name);
                    }
                    if (o is ManagerServer.Model.Transaction transaction)
                    {
                        var name = transaction.GetNameAndDescription();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            if (name.Contains(before))
                            {
                                before = name;
                            }
                            else
                            {
                                before += " — " + name;
                            }
                        }
                    }
                    else if (o is ManagerServer.Model.NamedObject namedObject)
                    {
                        var name = namedObject.GetName();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            if (name.Contains(before))
                            {
                                before = name;
                            }
                            else
                            {
                                before += " — " + name;
                            }
                        }
                    }
                }

                var after = string.Empty;
                if (ContentTypeAfter != Guid.Empty)
                {
                    var o = Serialization.Deserialize(ContentTypeAfter, ContentAfter);
                    if (o != null)
                    {
                        after = ManagerServer.Globalization.Strings.GetPropertyValue(o.GetType().Name);
                    }
                    if (o is ManagerServer.Model.Transaction transaction)
                    {
                        var name = transaction.GetNameAndDescription();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            if (name.Contains(after))
                            {
                                after = name;
                            }
                            else
                            {
                                after += " — " + name;
                            }
                        }
                    }
                    else if (o is ManagerServer.Model.NamedObject namedObject)
                    {
                        var name = namedObject.GetName();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            if (name.Contains(after))
                            {
                                after = name;
                            }
                            else
                            {
                                after += " — " + name;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(before) && !string.IsNullOrWhiteSpace(after) && !string.Equals(before, after))
                {
                    return $"<s>{before}</s><br />{after}";
                }
                else if (!string.IsNullOrWhiteSpace(after))
                {
                    return after;
                }
                else
                {
                    return before;
                }
            }
            [ManagerServer.Model.Attributes.TableColumn] public ChangeType GetAction()
            {
                if (IsCreatingChange) return ChangeType.Create;
                else if (IsUpdatingChange) return ChangeType.Update;
                else return ChangeType.Delete;
            }
        }

        [Table("Emails", WithoutRowId = true)]
        public class Email
        {
            [PrimaryKey] public Guid Key { get; set; }
            [Indexed] public Guid Object { get; set; }
            public string Sender { get; set; }
            public string Recipient { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public string Content { get; set; }
            public string Filename { get; set; }
            [Indexed] public long Timestamp { get; set; }
            public long Seen { get; set; }
            [Indexed] public string User { get; set; }

            [ManagerServer.Model.Attributes.TableColumn] public DateTime GetTimestamp() => new DateTime(Timestamp);
            [ManagerServer.Model.Attributes.TableColumn] public string GetSender() => User;
            [ManagerServer.Model.Attributes.TableColumn] public string GetRecipient() => Recipient;
            [ManagerServer.Model.Attributes.TableColumn] public string GetSubject() => Subject;
            [ManagerServer.Model.Attributes.TableColumn] public EmailStatus GetStatus() => (Seen == 0) ? EmailStatus.Sent : EmailStatus.Viewed;
        }

        public enum EmailStatus
        {
            [ManagerServer.Model.Attributes.Primary] Sent,
            [ManagerServer.Model.Attributes.Success] Viewed
        }

        public enum ChangeType
        {
            [ManagerServer.Model.Attributes.Primary] Create,
            [ManagerServer.Model.Attributes.Success] Update,
            [ManagerServer.Model.Attributes.Danger] Delete
        }

        public abstract class Action
        {
            public abstract Guid Key { get; }
        }

        public sealed class CreateOrUpdateAction : Action
        {
            public ManagerServer.Model.Object Object { get; private set; }
            public override Guid Key => Object.Key;

            public CreateOrUpdateAction(ManagerServer.Model.Object o)
            {
                Object = o;
            }
        }

        public sealed class DeleteAction : Action
        {
            private Guid key;
            public override Guid Key => key;

            public DeleteAction(Guid key)
            {
                this.key = key;
            }
        }
    }
}
