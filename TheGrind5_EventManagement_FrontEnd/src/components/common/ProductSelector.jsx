import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Card,
  CardContent,
  Checkbox,
  FormControlLabel,
  TextField,
  Grid,
  Avatar,
  Chip,
  Divider,
  Alert
} from '@mui/material';
import { ShoppingCart, AttachMoney } from '@mui/icons-material';
import { eventsAPI } from '../../services/apiClient';

const ProductSelector = ({ eventId, selectedProducts, onProductsChange }) => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        setError(null);
        
        // Check if user is authenticated
        const token = localStorage.getItem('token');
        if (!token) {
          setError('Bạn cần đăng nhập để xem sản phẩm phụ kiện');
          setLoading(false);
          return;
        }
        
        const response = await eventsAPI.getProductsForSelection(eventId);
        const productsData = response.data || [];
        
        // Initialize products with selection state
        const productsWithSelection = productsData.map(product => ({
          ...product,
          isSelected: selectedProducts.some(sp => sp.productId === product.productId),
          quantity: selectedProducts.find(sp => sp.productId === product.productId)?.quantity || 1
        }));
        
        setProducts(productsWithSelection);
      } catch (err) {
        console.error('Error fetching products:', err);
        console.error('Error details:', err.response?.data || err.message);
        
        // Handle different error types
        if (err.response?.status === 401) {
          setError('Bạn cần đăng nhập để xem sản phẩm phụ kiện');
        } else if (err.response?.status === 404) {
          setError('Không tìm thấy sản phẩm phụ kiện cho sự kiện này');
        } else {
          setError('Không thể tải danh sách sản phẩm phụ kiện');
        }
      } finally {
        setLoading(false);
      }
    };

    if (eventId) {
      fetchProducts();
    }
  }, [eventId, selectedProducts]);

  const handleProductToggle = (productId) => {
    const updatedProducts = products.map(product => {
      if (product.productId === productId) {
        return {
          ...product,
          isSelected: !product.isSelected,
          quantity: !product.isSelected ? 1 : 0
        };
      }
      return product;
    });
    
    setProducts(updatedProducts);
    
    // Update parent component
    const selectedProductsData = updatedProducts
      .filter(p => p.isSelected)
      .map(p => ({
        productId: p.productId,
        productName: p.productName,
        price: p.price,
        quantity: p.quantity
      }));
    
    onProductsChange(selectedProductsData);
  };

  const handleQuantityChange = (productId, quantity) => {
    const newQuantity = Math.max(1, parseInt(quantity) || 1);
    
    const updatedProducts = products.map(product => {
      if (product.productId === productId) {
        return { ...product, quantity: newQuantity };
      }
      return product;
    });
    
    setProducts(updatedProducts);
    
    // Update parent component
    const selectedProductsData = updatedProducts
      .filter(p => p.isSelected)
      .map(p => ({
        productId: p.productId,
        productName: p.productName,
        price: p.price,
        quantity: p.quantity
      }));
    
    onProductsChange(selectedProductsData);
  };

  const formatPrice = (price) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(price);
  };

  const calculateTotal = () => {
    return products
      .filter(p => p.isSelected)
      .reduce((total, product) => total + (product.price * product.quantity), 0);
  };

  if (loading) {
    return (
      <Box sx={{ p: 3, textAlign: 'center' }}>
        <Typography>Đang tải sản phẩm phụ kiện...</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ mb: 2 }}>
        {error}
      </Alert>
    );
  }

  if (products.length === 0) {
    return (
      <Alert severity="info" sx={{ mb: 2 }}>
        Sự kiện này không có sản phẩm phụ kiện nào.
      </Alert>
    );
  }

  return (
    <Box sx={{ mb: 3 }}>
      <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <ShoppingCart />
        Sản phẩm phụ kiện (Tùy chọn)
      </Typography>
      
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Chọn các sản phẩm phụ kiện bạn muốn mua cùng với vé
      </Typography>

      <Grid container spacing={2}>
        {products.map((product) => (
          <Grid item xs={12} sm={6} md={4} key={product.productId}>
            <Card 
              sx={{ 
                border: product.isSelected ? '2px solid' : '1px solid',
                borderColor: product.isSelected ? 'primary.main' : 'divider',
                '&:hover': { borderColor: 'primary.main' }
              }}
            >
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 2 }}>
                  <Avatar
                    src={product.productImage ? `http://localhost:5000${product.productImage}` : undefined}
                    sx={{ width: 60, height: 60 }}
                  >
                    <ShoppingCart />
                  </Avatar>
                  
                  <Box sx={{ flex: 1 }}>
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={product.isSelected}
                          onChange={() => handleProductToggle(product.productId)}
                        />
                      }
                      label={
                        <Box>
                          <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
                            {product.productName}
                          </Typography>
                          <Typography variant="h6" color="primary" sx={{ fontWeight: 700 }}>
                            {formatPrice(product.price)}
                          </Typography>
                        </Box>
                      }
                    />
                    
                    {product.description && (
                      <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                        {product.description}
                      </Typography>
                    )}
                    
                    {product.isSelected && (
                      <Box sx={{ mt: 2 }}>
                        <TextField
                          label="Số lượng"
                          type="number"
                          size="small"
                          value={product.quantity}
                          onChange={(e) => handleQuantityChange(product.productId, e.target.value)}
                          inputProps={{ min: 1, max: 10 }}
                          sx={{ width: 100 }}
                        />
                        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                          Tổng: {formatPrice(product.price * product.quantity)}
                        </Typography>
                      </Box>
                    )}
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {products.some(p => p.isSelected) && (
        <Box sx={{ mt: 3, p: 2, bgcolor: 'primary.light', borderRadius: 1 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Typography variant="h6" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <AttachMoney />
              Tổng cộng phụ kiện:
            </Typography>
            <Typography variant="h6" sx={{ fontWeight: 700 }}>
              {formatPrice(calculateTotal())}
            </Typography>
          </Box>
          
          <Box sx={{ mt: 1 }}>
            {products
              .filter(p => p.isSelected)
              .map(product => (
                <Chip
                  key={product.productId}
                  label={`${product.productName} x${product.quantity} - ${formatPrice(product.price * product.quantity)}`}
                  size="small"
                  sx={{ mr: 1, mb: 1 }}
                />
              ))
            }
          </Box>
        </Box>
      )}
    </Box>
  );
};

export default ProductSelector;
