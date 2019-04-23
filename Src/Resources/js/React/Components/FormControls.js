import React, { Component } from 'react';

export class TextArea extends Component {

    constructor(props) {
        super(props);
        this.state = {
            value: this.getValue()
        };
    

        this.handleChange = this.handleChange.bind(this);
    }

    getValue() {
        return this.props.value || "";
    }

    handleChange(event) {
        this.setState({ value: event.target.value });
    }

    render() {
        return (<textarea className="form-control" value={this.state.value} onChange={this.handleChange}></textarea>)
    }
}