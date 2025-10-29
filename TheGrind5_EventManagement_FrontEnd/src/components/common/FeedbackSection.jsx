import React, { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Typography,
  TextField,
  Button,
  Card,
  CardContent,
  Avatar,
  Stack,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  MenuItem,
  Select,
  FormControl,
  CircularProgress,
  Alert
} from '@mui/material';
import {
  ThumbUp,
  ThumbDown,
  Delete,
  Reply
} from '@mui/icons-material';
import { feedbackAPI } from '../../services/apiClient';

const FeedbackSection = ({ eventId }) => {
  const [feedbacks, setFeedbacks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [newComment, setNewComment] = useState('');
  const [replyingTo, setReplyingTo] = useState(null);
  const [replyText, setReplyText] = useState('');
  const [sortBy, setSortBy] = useState('newest');
  const [deleteDialog, setDeleteDialog] = useState(null);

  const fetchFeedbacks = useCallback(async () => {
    try {
      setLoading(true);
      const response = await feedbackAPI.getByEvent(eventId);
      setFeedbacks(response.data?.feedbacks || []);
    } catch (err) {
      console.error('Error fetching feedbacks:', err);
      setFeedbacks([]);
    } finally {
      setLoading(false);
    }
  }, [eventId]);

  useEffect(() => {
    fetchFeedbacks();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fetchFeedbacks, sortBy]);

  const handleSubmitComment = async () => {
    if (!newComment.trim()) return;

    // Check if user is logged in
    const token = localStorage.getItem('token');
    if (!token) {
      alert('Vui lòng đăng nhập để gửi comment');
      return;
    }

    const commentText = newComment;
    
    try {
      const response = await feedbackAPI.create({
        EventId: parseInt(eventId),
        Comment: commentText
      });
      
      console.log('Feedback created successfully:', response);
      
      // Clear input only on success
      setNewComment('');
      
      // Refresh feedbacks after successful creation
      await fetchFeedbacks();
    } catch (err) {
      console.error('========= FEEDBACK ERROR =========');
      console.error('Error object:', err);
      console.error('Error message:', err?.message);
      console.error('Error success:', err?.success);
      console.error('Error data:', err?.data);
      console.error('Original error:', err?.originalError);
      console.error('Response:', err?.originalError?.response);
      console.error('Response data:', err?.originalError?.response?.data);
      console.error('==================================');
      
      // Try to extract error message from various possible locations
      let errorMessage = 'Không thể gửi comment';
      
      if (err?.message && !err?.message.includes('Cannot read')) {
        errorMessage = err.message;
      } else if (err?.originalError?.response?.data?.message) {
        errorMessage = err.originalError.response.data.message;
      } else if (err?.originalError?.response?.data?.error) {
        errorMessage = err.originalError.response.data.error;
      }
      
      // Only show alert if there's a real error (not just a warning)
      if (errorMessage !== 'Không thể gửi comment' || !err?.success) {
        alert(`Lỗi: ${errorMessage}`);
      }
      
      // Keep comment text so user can try again
    }
  };

  const handleReply = async () => {
    if (!replyText.trim()) return;

    // Check if user is logged in
    const token = localStorage.getItem('token');
    if (!token) {
      alert('Vui lòng đăng nhập để phản hồi');
      return;
    }

    const replyTextCopy = replyText;
    const replyingToCopy = replyingTo;

    try {
      await feedbackAPI.createReply(replyingToCopy, replyTextCopy);
      
      // Clear reply input and close reply mode only on success
      setReplyingTo(null);
      setReplyText('');
      
      // Refresh feedbacks after successful creation
      fetchFeedbacks();
    } catch (err) {
      const errorMessage = err?.originalError?.response?.data?.message || 
                          err?.message || 
                          'Không thể gửi reply';
      console.error('Error replying:', err);
      alert(`Lỗi: ${errorMessage}`);
      // Keep reply mode and text so user can try again
    }
  };

  const handleReaction = async (feedbackId, reactionType) => {
    // Check if user is logged in
    const token = localStorage.getItem('token');
    if (!token) {
      alert('Vui lòng đăng nhập để like/dislike');
      return;
    }

    // Update reaction optimistically
    const updateFeedbackReaction = (feedbacks) => feedbacks.map(fb => {
      // Update main feedback
      if (fb.feedbackId === feedbackId) {
        const currentReaction = fb.stats?.userReaction;
        const newLikeCount = (fb.stats?.likeCount || 0) + 
          (reactionType === 'Like' ? (currentReaction === 'Like' ? -1 : currentReaction === 'Dislike' ? 1 : 1) : 0);
        const newDislikeCount = (fb.stats?.dislikeCount || 0) + 
          (reactionType === 'Dislike' ? (currentReaction === 'Dislike' ? -1 : currentReaction === 'Like' ? 1 : 1) : 0);
        
        return {
          ...fb,
          stats: {
            likeCount: Math.max(0, newLikeCount),
            dislikeCount: Math.max(0, newDislikeCount),
            userReaction: currentReaction === reactionType ? null : reactionType
          }
        };
      }
      
      // Check if it's a reply
      if (fb.replies && fb.replies.length > 0) {
        const updatedReplies = fb.replies.map(reply => {
          if (reply.feedbackId === feedbackId) {
            const currentReaction = reply.stats?.userReaction;
            const newLikeCount = (reply.stats?.likeCount || 0) + 
              (reactionType === 'Like' ? (currentReaction === 'Like' ? -1 : currentReaction === 'Dislike' ? 1 : 1) : 0);
            const newDislikeCount = (reply.stats?.dislikeCount || 0) + 
              (reactionType === 'Dislike' ? (currentReaction === 'Dislike' ? -1 : currentReaction === 'Like' ? 1 : 1) : 0);
            
            return {
              ...reply,
              stats: {
                likeCount: Math.max(0, newLikeCount),
                dislikeCount: Math.max(0, newDislikeCount),
                userReaction: currentReaction === reactionType ? null : reactionType
              }
            };
          }
          return reply;
        });
        
        return { ...fb, replies: updatedReplies };
      }
      
      return fb;
    });

    setFeedbacks(updateFeedbackReaction);

    try {
      await feedbackAPI.addReaction({
        FeedbackId: feedbackId,
        ReactionType: reactionType
      });
      // Re-fetch to ensure data is in sync
      fetchFeedbacks();
    } catch (err) {
      const errorMessage = err?.originalError?.response?.data?.message || 
                          err?.message || 
                          'Không thể thêm reaction';
      console.error('Error adding reaction:', errorMessage);
      // Revert on error
      fetchFeedbacks();
    }
  };

  const handleDelete = async (feedbackId) => {
    try {
      await feedbackAPI.delete(feedbackId);
      setDeleteDialog(null);
      fetchFeedbacks();
    } catch (err) {
      alert('Không thể xóa feedback');
      console.error('Error deleting feedback:', err);
    }
  };

  const sortedFeedbacks = [...feedbacks].sort((a, b) => {
    if (sortBy === 'newest') {
      return new Date(b.createdAt) - new Date(a.createdAt);
    } else if (sortBy === 'oldest') {
      return new Date(a.createdAt) - new Date(b.createdAt);
    }
    return 0;
  });

  const renderFeedback = (feedback, isReply = false) => {
    const currentUser = JSON.parse(localStorage.getItem('user') || '{}');
    const isOwner = feedback.userId === (currentUser.userId || currentUser.Id || currentUser.id);

    return (
      <Box key={feedback.feedbackId} sx={{ mb: 2, ml: isReply ? 4 : 0 }}>
        <Card variant="outlined">
          <CardContent>
            <Stack spacing={2}>
              <Stack direction="row" spacing={2} alignItems="center">
                <Avatar sx={{ bgcolor: 'primary.main' }}>
                  {(feedback.userName || 'U')[0].toUpperCase()}
                </Avatar>
                <Box sx={{ flex: 1 }}>
                  <Typography variant="subtitle2" fontWeight={600}>
                    {feedback.userName || 'Anonymous'}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {new Date(feedback.createdAt).toLocaleString('vi-VN')}
                  </Typography>
                </Box>
                {isOwner && (
                  <IconButton
                    size="small"
                    onClick={() => setDeleteDialog(feedback.feedbackId)}
                  >
                    <Delete fontSize="small" />
                  </IconButton>
                )}
              </Stack>

              <Typography variant="body2">{feedback.comment}</Typography>

              <Stack direction="row" spacing={1} alignItems="center">
                <IconButton
                  size="small"
                  onClick={() => handleReaction(feedback.feedbackId, 'Like')}
                  color={feedback.stats?.userReaction === 'Like' ? 'primary' : 'default'}
                >
                  <ThumbUp fontSize="small" />
                </IconButton>
                <Typography variant="caption">
                  {feedback.stats?.likeCount || 0}
                </Typography>

                <IconButton
                  size="small"
                  onClick={() => handleReaction(feedback.feedbackId, 'Dislike')}
                  color={feedback.stats?.userReaction === 'Dislike' ? 'error' : 'default'}
                >
                  <ThumbDown fontSize="small" />
                </IconButton>
                <Typography variant="caption">
                  {feedback.stats?.dislikeCount || 0}
                </Typography>

                {!isReply && (
                  <Button
                    size="small"
                    startIcon={<Reply />}
                    onClick={() => setReplyingTo(feedback.feedbackId)}
                  >
                    Trả lời
                  </Button>
                )}
              </Stack>

              {replyingTo === feedback.feedbackId && (
                <Box sx={{ mt: 1 }}>
                  <TextField
                    fullWidth
                    multiline
                    rows={2}
                    size="small"
                    placeholder="Viết phản hồi..."
                    value={replyText}
                    onChange={(e) => setReplyText(e.target.value)}
                  />
                  <Stack direction="row" spacing={1} sx={{ mt: 1 }}>
                    <Button
                      size="small"
                      variant="contained"
                      onClick={handleReply}
                    >
                      Gửi
                    </Button>
                    <Button
                      size="small"
                      onClick={() => {
                        setReplyingTo(null);
                        setReplyText('');
                      }}
                    >
                      Hủy
                    </Button>
                  </Stack>
                </Box>
              )}

              {feedback.replies && feedback.replies.length > 0 && (
                <Box>
                  {feedback.replies.map((reply) => renderFeedback(reply, true))}
                </Box>
              )}
            </Stack>
          </CardContent>
        </Card>
      </Box>
    );
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 3 }}>
        <Typography variant="h6" fontWeight={600}>
          Đánh giá và nhận xét ({feedbacks.length})
        </Typography>
        <FormControl size="small" sx={{ minWidth: 150 }}>
          <Select
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
          >
            <MenuItem value="newest">Mới nhất</MenuItem>
            <MenuItem value="oldest">Cũ nhất</MenuItem>
          </Select>
        </FormControl>
      </Stack>

      {/* Comment Input */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <TextField
            fullWidth
            multiline
            rows={3}
            placeholder={
              localStorage.getItem('token') 
                ? "Chia sẻ nhận xét của bạn về sự kiện này..." 
                : "Vui lòng đăng nhập để gửi nhận xét..."
            }
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
            disabled={!localStorage.getItem('token')}
            sx={{ mb: 2 }}
          />
          <Button
            variant="contained"
            onClick={handleSubmitComment}
            disabled={!newComment.trim() || !localStorage.getItem('token')}
          >
            Gửi nhận xét
          </Button>
        </CardContent>
      </Card>

      {/* Feedbacks List */}
      {feedbacks.length === 0 ? (
        <Alert severity="info">Chưa có nhận xét nào. Hãy là người đầu tiên chia sẻ!</Alert>
      ) : (
        <Box>
          {sortedFeedbacks.map((feedback) => renderFeedback(feedback))}
        </Box>
      )}

      {/* Delete Confirmation Dialog */}
      <Dialog open={!!deleteDialog} onClose={() => setDeleteDialog(null)}>
        <DialogTitle>Xác nhận xóa</DialogTitle>
        <DialogContent>
          <Typography>Bạn có chắc chắn muốn xóa nhận xét này?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialog(null)}>Hủy</Button>
          <Button onClick={() => handleDelete(deleteDialog)} color="error">
            Xóa
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default FeedbackSection;
