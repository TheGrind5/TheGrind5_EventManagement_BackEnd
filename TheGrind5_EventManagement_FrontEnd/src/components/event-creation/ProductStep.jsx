import React, { useState, useCallback, memo } from 'react';
import {
  Box,
  Typography,
  TextField,
  Card,
  CardContent,
  Button,
  Grid,
  IconButton,
  InputAdornment,
  useTheme,
  Divider,
  Alert
} from '@mui/material';
import { Add, Delete, CloudUpload, Image } from '@mui/icons-material';
import { eventsAPI } from '../../services/apiClient';

const ProductStep = ({ data, onChange }) => {
  const theme = useTheme();
  const [uploading, setUploading] = useState({});
  const [errors, setErrors] = useState({});

  const handleInputChange = useCallback((field, value) => {
    onChange({
      ...data,
      [field]: value
    });
  }, [data, onChange]);

  const handleProductChange = useCallback((index, field, value) => {
    const newProducts = [...(data.products || [])];
    newProducts[index] = {
      ...newProducts[index],
      [field]: value
    };
    handleInputChange('products', newProducts);
  }, [data, handleInputChange]);

  const addProduct = useCallback(() => {
    const newProducts = [...(data.products || [])];
    newProducts.push({
      productName: '',
      productImage: '',
      price: 0,
      description: ''
    });
    handleInputChange('products', newProducts);
  }, [data, handleInputChange]);

  const removeProduct = useCallback((index) => {
    const newProducts = [...(data.products || [])];
    newProducts.splice(index, 1);
    handleInputChange('products', newProducts);
  }, [data, handleInputChange]);

  const handleImageUpload = async (file, productIndex) => {
    try {
      setUploading(prev => ({ ...prev, [productIndex]: true }));
      
      // Validate file type and size
      if (!file.type.startsWith('image/')) {
        throw new Error('Chỉ được upload file ảnh');
      }
      
      if (file.size > 5 * 1024 * 1024) { // 5MB
        throw new Error('Kích thước file không được quá 5MB');
      }
      
      const response = await eventsAPI.uploadImage(file);
      
      // Validate response
      if (!response.data || !response.data.imageUrl) {
        throw new Error('Không nhận được URL ảnh từ server');
      }
      
      handleProductChange(productIndex, 'productImage', response.data.imageUrl);
    } catch (error) {
      console.error('Upload failed:', error);
      
      // Clear any existing error for this product
      const newErrors = { ...errors };
      delete newErrors[`upload_${productIndex}`];
      setErrors(newErrors);
      
      // Show user-friendly error message
      const errorMessage = error.message || 'Upload ảnh thất bại';
      alert(errorMessage);
    } finally {
      setUploading(prev => ({ ...prev, [productIndex]: false }));
    }
  };

  const validateProduct = (product, index) => {
    const newErrors = { ...errors };
    const productErrors = {};

    // Validate product name
    if (!product.productName || product.productName.trim().length === 0) {
      productErrors.productName = 'Tên sản phẩm không được để trống';
    } else if (product.productName.trim().length < 2) {
      productErrors.productName = 'Tên sản phẩm phải có ít nhất 2 ký tự';
    } else if (product.productName.trim().length > 200) {
      productErrors.productName = 'Tên sản phẩm không được quá 200 ký tự';
    }

    // Validate price
    if (product.price === undefined || product.price === null) {
      productErrors.price = 'Giá sản phẩm không được để trống';
    } else if (product.price < 0) {
      productErrors.price = 'Giá sản phẩm không được âm';
    } else if (product.price > 999999999) {
      productErrors.price = 'Giá sản phẩm quá cao';
    }

    // Validate description if provided
    if (product.description && product.description.length > 1000) {
      productErrors.description = 'Mô tả sản phẩm không được quá 1000 ký tự';
    }

    if (Object.keys(productErrors).length > 0) {
      newErrors[`product_${index}`] = productErrors;
    } else {
      delete newErrors[`product_${index}`];
    }

    setErrors(newErrors);
    return Object.keys(productErrors).length === 0;
  };

  const handleProductBlur = (product, index) => {
    validateProduct(product, index);
  };

  const formatPrice = (value) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(value);
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', p: 3 }}>
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 700, mb: 4 }}>
        Sản phẩm phụ kiện
      </Typography>

      <Alert severity="info" sx={{ mb: 3 }}>
        Thêm sản phẩm phụ kiện cho sự kiện của bạn (tùy chọn). Khách hàng có thể mua các sản phẩm này khi tham gia sự kiện.
      </Alert>

      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
        {/* Danh sách sản phẩm */}
        {(data.products || []).map((product, index) => (
          <Card key={index} sx={{ p: 3, border: '1px solid', borderColor: 'divider' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
              <Typography variant="h6" sx={{ fontWeight: 600 }}>
                Sản phẩm {index + 1}
              </Typography>
              <IconButton
                color="error"
                onClick={() => removeProduct(index)}
                size="small"
              >
                <Delete />
              </IconButton>
            </Box>

            <Grid container spacing={3}>
              {/* Tên sản phẩm */}
              <Grid item xs={12} md={6}>
                <TextField
                  label="Tên sản phẩm"
                  value={product.productName}
                  onChange={(e) => handleProductChange(index, 'productName', e.target.value)}
                  onBlur={() => handleProductBlur(product, index)}
                  fullWidth
                  required
                  placeholder="VD: Áo thun sự kiện, Mũ lưỡi trai, Túi tote..."
                  inputProps={{ maxLength: 100 }}
                  error={!!(errors[`product_${index}`]?.productName)}
                  helperText={errors[`product_${index}`]?.productName}
                />
              </Grid>

              {/* Giá sản phẩm */}
              <Grid item xs={12} md={6}>
                <TextField
                  label="Giá sản phẩm"
                  type="number"
                  value={product.price}
                  onChange={(e) => handleProductChange(index, 'price', parseFloat(e.target.value) || 0)}
                  onBlur={() => handleProductBlur(product, index)}
                  fullWidth
                  required
                  placeholder="0 (miễn phí nếu để 0)"
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <Typography variant="body2" color="text.secondary">
                          VND
                        </Typography>
                      </InputAdornment>
                    ),
                    endAdornment: (
                      <InputAdornment position="end">
                        <Typography variant="caption" color="text.secondary">
                          {formatPrice(product.price)}
                        </Typography>
                      </InputAdornment>
                    ),
                  }}
                  error={!!(errors[`product_${index}`]?.price)}
                  helperText={errors[`product_${index}`]?.price || "Để 0 nếu sản phẩm miễn phí"}
                />
              </Grid>

              {/* Mô tả sản phẩm */}
              <Grid item xs={12}>
                <TextField
                  label="Mô tả sản phẩm"
                  value={product.description || ''}
                  onChange={(e) => handleProductChange(index, 'description', e.target.value)}
                  onBlur={() => handleProductBlur(product, index)}
                  fullWidth
                  multiline
                  rows={3}
                  placeholder="Mô tả chi tiết về sản phẩm (tùy chọn)"
                  inputProps={{ maxLength: 1000 }}
                  error={!!(errors[`product_${index}`]?.description)}
                  helperText={errors[`product_${index}`]?.description || `${(product.description || '').length}/1000 ký tự`}
                />
              </Grid>

              {/* Hình ảnh sản phẩm */}
              <Grid item xs={12}>
                <Typography variant="subtitle2" gutterBottom sx={{ fontWeight: 600 }}>
                  Hình ảnh sản phẩm
                </Typography>
                <Card
                  sx={{
                    height: 200,
                    cursor: 'pointer',
                    border: '2px dashed',
                    borderColor: 'divider',
                    '&:hover': {
                      borderColor: 'primary.main',
                      bgcolor: 'action.hover'
                    }
                  }}
                  onClick={() => {
                    const input = document.createElement('input');
                    input.type = 'file';
                    input.accept = 'image/*';
                    input.onchange = (e) => {
                      const file = e.target.files[0];
                      if (file) {
                        handleImageUpload(file, index);
                      }
                    };
                    input.click();
                  }}
                >
                  {product.productImage ? (
                    <Box sx={{ position: 'relative', width: '100%', height: '100%' }}>
                      <img
                        src={`http://localhost:5000${product.productImage}`}
                        alt="Product Preview"
                        style={{
                          width: '100%',
                          height: '100%',
                          objectFit: 'cover'
                        }}
                      />
                      <Button
                        size="small"
                        variant="contained"
                        color="error"
                        sx={{
                          position: 'absolute',
                          top: 8,
                          right: 8,
                          minWidth: 'auto',
                          width: 24,
                          height: 24,
                          borderRadius: '50%',
                          padding: 0
                        }}
                        onClick={(e) => {
                          e.stopPropagation();
                          handleProductChange(index, 'productImage', '');
                        }}
                      >
                        ×
                      </Button>
                    </Box>
                  ) : (
                    <CardContent sx={{ textAlign: 'center', height: '100%', display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                      <CloudUpload sx={{ fontSize: 48, color: 'primary.main', mb: 1 }} />
                      <Typography variant="body2" color="text.secondary">
                        Thêm hình ảnh sản phẩm
                      </Typography>
                      <Button
                        size="small"
                        variant="outlined"
                        sx={{ mt: 2 }}
                        disabled={uploading[index]}
                        onClick={(e) => {
                          e.stopPropagation();
                          const input = document.createElement('input');
                          input.type = 'file';
                          input.accept = 'image/*';
                          input.onchange = (e) => {
                            const file = e.target.files[0];
                            if (file) {
                              handleImageUpload(file, index);
                            }
                          };
                          input.click();
                        }}
                      >
                        {uploading[index] ? 'Đang upload...' : 'Chọn ảnh'}
                      </Button>
                    </CardContent>
                  )}
                </Card>
              </Grid>
            </Grid>
          </Card>
        ))}

        {/* Nút thêm sản phẩm */}
        <Card sx={{ p: 3, border: '2px dashed', borderColor: 'primary.main' }}>
          <Button
            variant="outlined"
            color="primary"
            startIcon={<Add />}
            onClick={addProduct}
            fullWidth
            sx={{ py: 2 }}
          >
            Thêm sản phẩm phụ kiện
          </Button>
        </Card>

        {/* Thông báo nếu không có sản phẩm */}
        {(!data.products || data.products.length === 0) && (
          <Alert severity="info">
            Bạn có thể bỏ qua bước này nếu không có sản phẩm phụ kiện nào. Nhấn "Tiếp tục" để chuyển sang bước tiếp theo.
          </Alert>
        )}
      </Box>
    </Box>
  );
};

export default memo(ProductStep);
